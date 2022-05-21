using AElf;
using AElf.Client.MultiToken;
using AElf.Client.Proto;

namespace Client.Infrastructure.Models
{
    public class TokenInfoWithBalance
    {
        public bool IsNative { get; set; }
        public string Symbol { get; set; }
        public string TokenName { get; set; }
        public int Decimals { get; set; }
        public int IssueChainId { get; set; }
        public string IssueChainIdBase58 => ChainHelper.ConvertChainIdToBase58(IssueChainId);
        public Address Issuer { get; set; }
        public long TotalSupply { get; set; }
        public long? MainChainSupply { get; set; }
        public long? SideChainSupply { get; set; }
        public long? MainChainBalance { get; set; }
        public long? SideChainBalance { get; set; }
    }
}
