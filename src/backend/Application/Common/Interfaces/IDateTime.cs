using System;

namespace Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }
        DateTimeOffset UtcNowOffset { get; }
    }
}