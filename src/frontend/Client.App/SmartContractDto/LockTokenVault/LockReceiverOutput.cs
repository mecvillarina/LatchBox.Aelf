using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockReceiverOutput
    {
        public string Receiver { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Amount { get; set; }

        public TimestampOutput DateClaimed { get; set; }
        public TimestampOutput DateRevoked { get; set; }

        public bool IsActive { get; set; }
    }
}
