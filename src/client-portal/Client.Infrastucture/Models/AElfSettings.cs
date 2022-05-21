namespace Client.Infrastructure.Models
{
    public class AElfSettings
    {
        public string Network { get; set; }
        public string Node { get; set; }
        public string PlatformTokenSymbol { get; set; }
        public string MultiTokenContractAddress { get; set; }
        public string FaucetContractAddress { get; set; }
        public string MultiCrowdSaleContractAddress { get; set; }
        public string LockTokenVaultContractAddress { get; set; }
        public string VestingTokenVaultContractAddress { get; set; }
    }
}
