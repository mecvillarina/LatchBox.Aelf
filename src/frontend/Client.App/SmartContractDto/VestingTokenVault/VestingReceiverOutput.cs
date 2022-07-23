using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingReceiverOutput
    {
        public string Name { get; set; }
        public string Address { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Amount { get; set; }
        public TimestampOutput DateClaimed { get; set; }
        public TimestampOutput DateRevoked { get; set; }

    }
}