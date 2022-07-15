using Application.Common.Mappings;
using Domain.Entities;
using System;

namespace Application.Common.Dtos
{
    public class ChainInfoDto : IMapFrom<ChainInfo>
    {
        public string ChainIdBase58 { get; set; }
        public int ChainId { get; set; }
        public string RpcApi { get; set; }
        public string Explorer { get; set; }
        public string ChainType { get; set; }
        public long LongestChainHeight { get; set; }
        public string LongestChainHash { get; set; }
        public string GenesisBlockHash { get; set; }
        public string GenesisContractAddress { get; set; }
        public string LastIrreversibleBlockHash { get; set; }
        public long LastIrreversibleBlockHeight { get; set; }
        public string BestChainHash { get; set; }
        public long BestChainHeight { get; set; }

        public DateTime? LastUpdate { get; set; }

        public string TokenContractAddress { get; set; }
        public bool IsTokenCreationFeatureSupported { get; set; }
        public bool IsLockingFeatureSupported { get; set; }
        public bool IsVestingFeatureSupported { get; set; }
    }
}
