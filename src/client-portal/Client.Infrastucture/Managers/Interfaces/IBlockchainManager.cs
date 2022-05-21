using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IBlockchainManager : IManager
    {
        string Network { get; }
        string Node { get; }
        Task<string> GetChainIdAsync();
        Task<int> GetChainIdIntValueAsync();
    }
}