using Application.Common.Interfaces;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class MutableCallContext : ICallContext
    {
        public Guid CorrelationId { get; set; }
        public string AuthenticationType { get; set; }
        public string FunctionName { get; set; }
        public IDictionary<string, string> AdditionalProperties { get; } = new Dictionary<string, string>();
    }
}