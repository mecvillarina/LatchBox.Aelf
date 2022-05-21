using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IBlockchainManager : IManager
    {
        string MainChainNode { get; }
        string MainChainExplorer { get; }
        string SideChainNode { get; }
        string SideChainExplorer { get; }
        Task<string> GetMainChainIdInformationAsync();
        Task<string> GetSideChainIdInformationAsync();
    }
}