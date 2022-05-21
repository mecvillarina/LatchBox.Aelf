using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IBlockchainManager : IManager
    {
        string Node { get; }
        Task<string> GetChainIdAsync();
        Task<int> GetChainIdIntValueAsync();
    }
}