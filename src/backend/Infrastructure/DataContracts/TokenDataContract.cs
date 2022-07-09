using System.Text.Json.Serialization;

namespace Infrastructure.DataContracts
{
    public class TokenDataContract
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("contractAddress")]
        public string ContractAddress { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("chainId")]
        public string ChainId { get; set; }

        [JsonPropertyName("issueChainId")]
        public string IssueChainId { get; set; }

        [JsonPropertyName("txId")]
        public string TxId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("totalSupply")]
        public string TotalSupply { get; set; }

        [JsonPropertyName("supply")]
        public string Supply { get; set; }

        [JsonPropertyName("decimals")]
        public long Decimals { get; set; }

        [JsonPropertyName("holders")]
        public long Holders { get; set; }
    }
}
