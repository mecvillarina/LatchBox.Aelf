using Client.Infrastructure.Services.Interfaces;
using System;

namespace Client.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    }
}