using Client.App.Parameters;
using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto
{
    public class TokenCreateTokenInput
    {
        //[JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        //[JsonPropertyName("token_name")]
        public string TokenName { get; set; }

        //[JsonPropertyName("total_supply")]
        public long TotalSupply { get; set; }

        //[JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        //[JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        //[JsonPropertyName("is_burnable")]
        public bool IsBurnable { get; set; }

        //[JsonPropertyName("issue_chain_id")]
        public int IssueChainId { get; set; }

        public TokenCreateTokenInput(CreateTokenParameter parameter)
        {
            Symbol = parameter.Symbol;
            TokenName = parameter.TokenName;
            TotalSupply = parameter.TotalSupply;
            Decimals = parameter.Decimals;
            IsBurnable = parameter.IsBurnable;
            IssueChainId = parameter.IssueChainId;
        }
    }
}
