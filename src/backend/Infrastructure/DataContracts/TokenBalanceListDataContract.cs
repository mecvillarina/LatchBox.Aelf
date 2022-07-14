using System.Text.Json.Serialization;

namespace Infrastructure.DataContracts
{
    public class TokenBalanceListDataContract
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("balance")]
        public string Balance { get; set; }
    }
}
