using System;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions
{
    [Serializable]
    public class RpcServerErrorException : Exception
    {
        public RpcServerErrorException(string message)
                : base(message)
        {
        }

        protected RpcServerErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
