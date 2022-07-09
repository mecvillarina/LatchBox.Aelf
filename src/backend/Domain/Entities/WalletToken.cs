using Domain.Common;

namespace Domain.Entities
{
    public class WalletToken : AuditableEntity
    {
        public int Id { get; set; }
        public string ChainIdBase58 { get; set; }
        public string WalletAddress { get; set; }
        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }
        public int TokenDecimals { get; set; }
        public string TokenSupply { get; set; }
        public string TokenTotalSupply { get; set; }
    }
}
