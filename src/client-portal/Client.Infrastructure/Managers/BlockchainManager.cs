using AElf;
using AElf.Client.Dto;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class BlockchainManager : ManagerBase, IBlockchainManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly IMemoryCache _cache;
        public BlockchainManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IMemoryCache cache) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _cache = cache;
        }

        public string MainChainNode => ManagerToolkit.AelfSettings.MainChainNode;
        public string MainChainExplorer => ManagerToolkit.AelfSettings.MainChainExplorer;
        public string SideChainNode => ManagerToolkit.AelfSettings.SideChainNode;
        public string SideChainExplorer => ManagerToolkit.AelfSettings.SideChainExplorer;

        public int GetMainChainId()
        {
            var chainStatus = FetchMainChainStatus();

            if (chainStatus == null) return 0;

            return ChainHelper.ConvertBase58ToChainId(chainStatus.ChainId);
        }

        public int GetSideChainId()
        {
            var chainStatus = FetchSideChainStatus();

            if (chainStatus == null) return 0;

            return ChainHelper.ConvertBase58ToChainId(chainStatus.ChainId);
        }

        public ChainStatusDto FetchMainChainStatus()
        {
            return _cache.Get<ChainStatusDto>("MainChainStatus");
        }

        public async Task<ChainStatusDto> GetMainChainStatusAsync()
        {
            var chainStatus = await _blockChainService.GetMainChainStatusAsync();

            if (chainStatus != null)
            {
                _cache.Set("MainChainStatus", chainStatus);
            }
            else
            {
                chainStatus = FetchSideChainStatus();
            }

            return chainStatus;
        }

        public ChainStatusDto FetchSideChainStatus()
        {
            return _cache.Get<ChainStatusDto>("SideChainStatus");
        }

        public async Task<ChainStatusDto> GetSideChainStatusAsync()
        {
            var chainStatus = await _blockChainService.GetSideChainStatusAsync();

            if (chainStatus != null)
            {
                _cache.Set("SideChainStatus", chainStatus);
            }
            else
            {
                chainStatus = FetchSideChainStatus();
            }

            return chainStatus;
        }
    }
}
