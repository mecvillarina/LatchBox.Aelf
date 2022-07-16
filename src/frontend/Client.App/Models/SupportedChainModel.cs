using Application.Common.Dtos;

namespace Client.App.Models
{
    public class SupportedChainModel
    {
        public SupportedChainModel(ChainInfoDto chain)
        {
            ChainType = chain.ChainType;
            ChainIdBase58 = chain.ChainIdBase58;
            ExplorerUrl = chain.Explorer;
            ChainId = chain.ChainId;
        }

        public int ChainId { get; private set; }
        public string ChainType { get; private set; }
        public string ChainIdBase58 { get; private set; }
        public string ExplorerUrl { get; private set; }
    }
}
