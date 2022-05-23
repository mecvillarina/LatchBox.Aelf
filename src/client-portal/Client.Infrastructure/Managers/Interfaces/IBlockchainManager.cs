using AElf.Client.Dto;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IBlockchainManager : IManager
    {
        string MainChainNode { get; }
        string MainChainExplorer { get; }
        string SideChainNode { get; }
        string SideChainExplorer { get; }
        int GetMainChainId();
        int GetSideChainId();
        ChainStatusDto FetchMainChainStatus();
        Task<ChainStatusDto> GetMainChainStatusAsync();
        ChainStatusDto FetchSideChainStatus();
        Task<ChainStatusDto> GetSideChainStatusAsync();
    }
}