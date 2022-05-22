using AElf;
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

        public string MainChainNode => ManagerToolkit.AelfSettings.MainChainNode;
        public string MainChainExplorer => ManagerToolkit.AelfSettings.MainChainExplorer;
        public string SideChainNode => ManagerToolkit.AelfSettings.SideChainNode;
        public string SideChainExplorer => ManagerToolkit.AelfSettings.SideChainExplorer;

        public async Task<int> GetMainChainIdAsync()
        {
            return await _blockChainService.GetMainChainIdAsync();
        }

        public async Task<int> GetSideChainIdAsync()
        {
            return await _blockChainService.GetSideChainIdAsync();
        }
    }
}
