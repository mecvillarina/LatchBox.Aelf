using Newtonsoft.Json;

namespace Application.Common.Dtos
{
    public class TokenDto
    {
        public long Id { get; set; }
        public string ContractAddress { get; set; }
        public string Symbol { get; set; }
        public string ChainId { get; set; }
        public string IssueChainId { get; set; }
        public string TxId { get; set; }
        public string Name { get; set; }
        public string TotalSupply { get; set; }
        public string Supply { get; set; }
        public long Decimals { get; set; }
        public string Issuer { get; set; }

    }
}
