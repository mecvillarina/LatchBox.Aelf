using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingPeriodOutput
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long PeriodId { get; set; }
        public string Name { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TotalAmount { get; set; }
        public TimestampOutput UnlockTime { get; set; }
    }
}
