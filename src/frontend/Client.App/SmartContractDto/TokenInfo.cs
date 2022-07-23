using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto
{
    public class TokenInfo
    {
        public string Symbol { get; set; }
        public string TokenName { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Supply { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TotalSupply { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Decimals { get; set; }

        public string Issuer { get; set; }
        public bool IsBurnable { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int IssueChainId { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Issued { get; set; }
    }
}
