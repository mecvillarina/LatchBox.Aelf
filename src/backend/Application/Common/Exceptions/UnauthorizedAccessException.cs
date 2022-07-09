using System;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions
{
    [Serializable]
    public class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException()
        {
        }

        public UnauthorizedAccessException(string message) : base(message)
        {
        }

        public UnauthorizedAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnauthorizedAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}