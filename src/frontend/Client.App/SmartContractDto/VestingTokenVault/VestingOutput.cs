using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingOutput
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long VestingId { get; set; }
        public string TokenSymbol { get; set; }
        public string Initiator { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TotalAmount { get; set; }
        public TimestampOutput CreationTime { get; set; }
        public bool IsRevocable { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsActive { get; set; }
    }
}
