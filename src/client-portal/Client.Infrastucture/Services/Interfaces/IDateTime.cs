using System;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }
        DateTimeOffset UtcNowOffset { get; }
    }
}