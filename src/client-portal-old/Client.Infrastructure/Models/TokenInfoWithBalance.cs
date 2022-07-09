using AElf;
using AElf.Client.MultiToken;
using AElf.Client.Proto;
using Client.Infrastructure.Extensions;

namespace Client.Infrastructure.Models
{
    public class TokenInfoBase
    {
        public string Symbol { get; set; }
        public string TokenName { get; set; }
        public int Decimals { get; set; }
        public int IssueChainId { get; set; }
        public string IssueChainIdBase58 => ChainHelper.ConvertChainIdToBase58(IssueChainId);
        public string IssuerAddress { get; set; }
        public long TotalSupply { get; set; }

        public TokenInfoBase()
        {

        }
        public TokenInfoBase(TokenInfoBase tokenInfo)
        {
            Symbol = tokenInfo.Symbol;
            TokenName = tokenInfo.TokenName;
            Decimals = tokenInfo.Decimals;
            IssueChainId = tokenInfo.IssueChainId;
            IssuerAddress = tokenInfo.IssuerAddress;
            TotalSupply = tokenInfo.TotalSupply;
        }

        public TokenInfoBase(TokenInfo tokenInfo)
        {
            Symbol = tokenInfo.Symbol;
            TokenName = tokenInfo.TokenName;
            Decimals = tokenInfo.Decimals;
            IssueChainId = tokenInfo.IssueChainId;
            IssuerAddress = tokenInfo.Issuer.ToStringAddress();
            TotalSupply = tokenInfo.TotalSupply;
        }
    }

    public class TokenInfoWithBalance : TokenInfoBase
    {
        public TokenInfoWithBalance(TokenInfoBase tokenInfo) : base(tokenInfo)
        {
        }

        public bool IsNative { get; set; }
        public long? MainChainSupply { get; set; }
        public long? SideChainSupply { get; set; }
        public long? MainChainBalance { get; set; }
        public long? SideChainBalance { get; set; }
    }
}
