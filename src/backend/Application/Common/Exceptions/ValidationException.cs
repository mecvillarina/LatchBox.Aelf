using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Results;

namespace Application.Common.Exceptions
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException()
                : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(List<ValidationFailure> failures)
                : this()
        {
            var failureGroups = failures
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}