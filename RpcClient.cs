using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RFRabbitMQ;
using RFRabbitMQRpcClient.Types;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RFRabbitMQRpcClient
{
    public class RpcClient
        : IAsyncDisposable
    {
        private RabbitMQOptions Options { get; }
        private ConnectionFactory ConnectionFactory { get; }
        private ConcurrentDictionary<string, TaskCompletionSource<Response>> CallbackMapper { get; } = [];

        public RpcClient(RabbitMQOptions options)
        {
            Options = options;
            ConnectionFactory = new ConnectionFactory
            {
                HostName = Options.HostName,
                Port = Options.Port,
                Ssl = Options.Ssl,
                UserName = Options.UserName,
                Password = Options.Password,
            };
        }

        public static RpcClient Create(RabbitMQOptions options)
            => new(options);

        private IConnection? Connection;
        private IChannel? Channel;
        private string? ReplyQueueName;

        public async Task StartAsync()
        {
            Connection = await ConnectionFactory.CreateConnectionAsync();
            Channel = await Connection.CreateChannelAsync();

            QueueDeclareOk queueDeclareResult = await Channel.QueueDeclareAsync();
            ReplyQueueName = queueDeclareResult.QueueName;
            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.ReceivedAsync += (model, ea) =>
            {
                string? correlationId = ea.BasicProperties.CorrelationId;
                if (!string.IsNullOrEmpty(correlationId)
                    && CallbackMapper.TryRemove(correlationId, out var tcs)
                )
                    tcs.TrySetResult(new Response(ea.Body.ToArray()));

                return Task.CompletedTask;
            };

            await Channel.BasicConsumeAsync(ReplyQueueName, true, consumer);
        }

        public async Task<Response> CallAsync(string routingKey, object? data = null, CancellationToken cancellationToken = default)
        {
            if (Channel is null)
            {
                throw new InvalidOperationException();
            }

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = ReplyQueueName
            };

            var tcs = new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously);
            CallbackMapper.TryAdd(correlationId, tcs);

            var request = data switch
            {
                null => null,
                string str => new Request(str),
                byte[] bytes => new Request(bytes),
                Request req => req,
                DataTransfer dat => dat,
                _ => new DataTransfer(data)
            };

            await Channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: request?.Data,
                cancellationToken: CancellationToken.None
            );

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    CallbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });

            return await tcs.Task;
        }

        public async Task<T> CallAsync<T>(string routingKey, object? data = null, CancellationToken cancellationToken = default)
        {
            var response = await CallAsync(routingKey, data, cancellationToken);
            var jsonResult = response.ToString();
            var result = JsonSerializer.Deserialize<T>(jsonResult);
            if (result == null)
            {
                var type = typeof(T);
                if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
                    throw new RpcException(500, "Invalid format response.");
            }

            return result!;
        }

        public async ValueTask DisposeAsync()
        {
            if (Channel is not null)
                await Channel.CloseAsync();

            if (Connection is not null)
                await Connection.CloseAsync();

            GC.SuppressFinalize(this);
        }

        public async Task<object?> CallOkAsync(
            string routingKey,
            object? data = null,
            string errorType = "Error",
            string? errorMessage = null,
            int errorStatusCode = 500,
            CancellationToken cancellationToken = default
        )
        {
            var result = await CallAsync<Result>(routingKey, data, cancellationToken);
            if (!result.Ok)
            {
                if (!string.IsNullOrEmpty(result.Error))
                    errorType = result.Error;

                if (!string.IsNullOrEmpty(result.Message))
                    errorMessage = result.Message;

                if (result.StatusCode != null)
                    errorStatusCode = result.StatusCode.Value;

                if (!string.IsNullOrEmpty(errorMessage))
                    throw new RpcException(errorType, errorStatusCode, errorMessage);

                throw new RpcException(
                    errorType,
                    errorStatusCode,
                    "Error getting response for {0} queue.", routingKey
                );
            }

            return result.Value;
        }
    }
}
