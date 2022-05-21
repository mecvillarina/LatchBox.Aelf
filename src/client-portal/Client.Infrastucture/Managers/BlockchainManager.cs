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

        public async Task<string> GetMainChainIdInformationAsync()
        {
            var value = await _blockChainService.GetMainChainIdAsync();
            return $"{ChainHelper.ConvertChainIdToBase58(value)} ({value})";
        }

        public async Task<string> GetSideChainIdInformationAsync()
        {
            var value = await _blockChainService.GetSideChainIdAsync();
            return $"{ChainHelper.ConvertChainIdToBase58(value)} ({value})";
        }
    }
}
