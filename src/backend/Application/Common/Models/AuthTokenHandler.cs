using System;

namespace Application.Common.Models
{
    public class AuthTokenHandler
    {
        public string Token { get; set; }
        public long ExpireAt { get; set; }

        public bool IsValid()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return DateTime.UtcNow < epoch.AddSeconds(ExpireAt);
        }
    }
}