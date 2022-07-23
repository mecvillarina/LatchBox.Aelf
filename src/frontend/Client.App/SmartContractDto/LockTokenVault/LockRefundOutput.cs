using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockRefundOutput
    {
        public string TokenSymbol { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Amount { get; set; }
    }
}
