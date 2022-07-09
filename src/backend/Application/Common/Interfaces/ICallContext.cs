using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Common.Interfaces
{
    public interface ICallContext
    {
        Guid CorrelationId { get; set; }

        string FunctionName { get; set; }

        string AuthenticationType { get; set; }

        IDictionary<string, string> AdditionalProperties { get; }
    }
}