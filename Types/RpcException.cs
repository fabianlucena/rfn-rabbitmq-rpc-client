namespace RFRabbitMQRpcClient.Types
{
    public class RpcException
        : Exception
    {
        public string Error { get; private set; }
        public int StatusCode { get; private set; }
        public string FormatMessage { get; private set; }
        public string[] ParamsList { get; private set; }
        override public string Message { get => GetMessage(); }

        public RpcException(string error, int statusCode, string formatMessage = "", params string[] paramsList)
        {
            Error = error;
            StatusCode = statusCode;
            FormatMessage = formatMessage;
            ParamsList = paramsList ?? [];
        }

        public RpcException(int statusCode, string formatMessage = "", params string[] paramsList)
        {
            Error = GetType().Name;
            StatusCode = statusCode;
            FormatMessage = formatMessage;
            ParamsList = paramsList ?? [];
        }

        public string GetMessage()
        {
            if (FormatMessage == "" || ParamsList.Length == 0)
                return FormatMessage;
            else
            {
                var message = FormatMessage;
                for (int i = 0; i < ParamsList.Length; i++)
                    message = message.Replace($"{{{i}}}", ParamsList[i]);

                return message;
            }
        }
    }
}
