namespace RFRabbitMQRpcClient.Types
{
    public class Request
        : DataTransfer
    {
        public Request(byte[] data)
            : base(data) { }

        public Request(string data)
            : base(data) { }
    }
}
