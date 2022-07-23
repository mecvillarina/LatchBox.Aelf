using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingRefundOutput
    {
        public string TokenSymbol { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Amount { get; set; }
    }
}
