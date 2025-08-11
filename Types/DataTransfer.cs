using System.Text;
using System.Text.Json;

namespace RFRabbitMQRpcClient.Types
{
    public class DataTransfer
    {
        public byte[] Data { get; }

        public DataTransfer(byte[] data)
            => Data = data;

        public DataTransfer(string data)
            => Data = Encoding.UTF8.GetBytes(data);

        public DataTransfer(object? data)
        {
            if (data == null)
            {
                Data = Encoding.UTF8.GetBytes("null");
            }
            else
            {
                var json = JsonSerializer.Serialize(data);
                Data = Encoding.UTF8.GetBytes(json);
            }
        }

        public string GetString()
            => Encoding.UTF8.GetString(Data ?? []);

        public override string ToString()
            => GetString();

        public byte[] GetBytes()
            => Data;
    }
}
