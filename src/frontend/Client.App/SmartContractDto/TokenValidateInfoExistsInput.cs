using Client.App.Parameters;

namespace Client.App.SmartContractDto
{
    public class TokenValidateInfoExistsInput
    {
        public string Symbol { get; set; }
        public string TokenName { get; set; }
        public long TotalSupply { get; set; }
        public int Decimals { get; set; }
        public string Issuer { get; set; }
        public bool IsBurnable { get; set; }
        public int IssueChainId { get; set; }

        public TokenValidateInfoExistsInput()
        {

        }

        public TokenValidateInfoExistsInput(CreateTokenParameter parameter)
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
