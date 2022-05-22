namespace Client.Infrastructure.Models
{
    public class AElfSettings
    {
        public string MainChainNode { get; set; }
        public string SideChainNode { get; set; }
        public string MainChainExplorer { get; set; }
        public string SideChainExplorer { get; set; }
        public string PlatformTokenSymbol { get; set; }
        public string MultiTokenContractAddress { get; set; }
        public string FaucetContractAddress { get; set; }
        public string MultiCrowdSaleContractAddress { get; set; }
        public string LockTokenVaultContractAddress { get; set; }
        public string VestingTokenVaultContractAddress { get; set; }
    }
}
