namespace RFRabbitMQRpcClient.Types
{
    public class Response
        : DataTransfer
    {
        public Response(byte[] data)
            : base (data) { }

        public Response(string data)
            : base(data) { }

        public Response(object? data)
            : base(data) { }
    }
}
