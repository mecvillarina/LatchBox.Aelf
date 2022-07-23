using Google.Protobuf.WellKnownTypes;
using System;
using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto
{
    public class TimestampOutput
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Seconds { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Nanos { get; set; }
    
        public DateTimeOffset GetUniversalDateTime()
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixEpoch.AddSeconds(Seconds).AddTicks(Nanos / Duration.NanosecondsPerTick);
        }
    }
}
