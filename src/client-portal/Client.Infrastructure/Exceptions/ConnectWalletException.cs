using System;
using System.Runtime.Serialization;

namespace Client.Infrastructure.Exceptions
{
    [Serializable]
    public class ConnectWalletException : Exception
    {
        public ConnectWalletException(string message, string messageTitle) : base(message)
        {
            MessageTitle = messageTitle;
        }

        public ConnectWalletException(string message, string messageTitle, Exception innerException) : base(message, innerException)
        {
            MessageTitle = messageTitle;
        }

        public ConnectWalletException()
        {
        }

        public ConnectWalletException(string message) : base(message)
        {
        }

        public ConnectWalletException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConnectWalletException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string MessageTitle { get; }
    }
}
