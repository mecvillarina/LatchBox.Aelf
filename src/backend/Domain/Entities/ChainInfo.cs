using System;

namespace Domain.Entities
{
    public class ChainInfo
    {
        public int Id { get; set; }
        public string ChainIdBase58 { get; set; }
        public int ChainId { get; set; }
        public string RpcApi { get; set; }
        public string Explorer { get; set; }
        public string ChainType { get; set; } // Main | Side

        public long LongestChainHeight { get; set; }
        public string LongestChainHash { get; set; }
        public string GenesisBlockHash { get; set; }
        public string GenesisContractAddress { get; set; }
        public string LastIrreversibleBlockHash { get; set; }
        public long LastIrreversibleBlockHeight { get; set; }
        public string BestChainHash { get; set; }
        public long BestChainHeight { get; set; }

        public bool IsEnabled { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int OrderIdx { get; set; }

        public string TokenContractAddress { get; set; }
        public bool IsTokenCreationFeatureSupported { get; set; }
        public bool IsLockingFeatureSupported { get; set; }
        public bool IsVestingFeatureSupported { get; set; }
        public bool IsLaunchpadFeatureSupported { get; set; }
        public string LockVaultContractAddress { get; set; }
        public string VestingVaultContractAddress { get; set; }
        public string LaunchpadContractAddress { get; set; }
    }
}
