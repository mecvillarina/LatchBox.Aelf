using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class BlockchainManager : ManagerBase, IBlockchainManager
    {
        private readonly IBlockChainService _blockChainService;
        public BlockchainManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
        }

        public string Network => ManagerToolkit.AelfSettings.Network;
        public string Node => ManagerToolkit.AelfSettings.Node;

        public Task<int> GetChainIdAsync() => _blockChainService.GetChainIdAsync();
    }
}
