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

            if (chainStatus == null) return 9992731;

            return ChainHelper.ConvertBase58ToChainId(chainStatus.ChainId);
        }

        public int GetSideChainId()
        {
            var chainStatus = FetchSideChainStatus();

            if (chainStatus == null) return 1866392;

            return ChainHelper.ConvertBase58ToChainId(chainStatus.ChainId);
        }

        public ChainStatusDto FetchMainChainStatus()
        {
            return _cache.Get<ChainStatusDto>("MainChainStatus");
        }

        public async Task<ChainStatusDto> GetMainChainStatusAsync()
        {
            var chainStatus = await _blockChainService.GetMainChainStatusAsync().ConfigureAwait(false);

            if (chainStatus != null)
            {
                _cache.Set("MainChainStatus", chainStatus);
            }
            else
            {
                chainStatus = FetchMainChainStatus();
            }

            return chainStatus;
        }

        public ChainStatusDto FetchSideChainStatus()
        {
            return _cache.Get<ChainStatusDto>("SideChainStatus");
        }

        public async Task<ChainStatusDto> GetSideChainStatusAsync()
        {
            var chainStatus = await _blockChainService.GetSideChainStatusAsync().ConfigureAwait(false);

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

        public async Task<string> GetMainChainTokenAddressAsync()
        {
            var contractAddress = await _blockChainService.GetMainChainContractAddressAsync(ManagerToolkit.AelfSettings.MultiTokenContractAddress).ConfigureAwait(false);

            if (ManagerToolkit.AelfSettings.MultiTokenContractAddress != contractAddress)
            {
                _cache.Set("MainChainTokenAddress", contractAddress);
            }

            return contractAddress;
        }

        public string FetchMainChainTokenAddress()
        {
            var contractAddress = _cache.Get<string>("MainChainTokenAddress");

            if (string.IsNullOrEmpty(contractAddress))
            {
                contractAddress = ManagerToolkit.AelfSettings.MultiTokenContractAddress;
            }

            return contractAddress;
        }

        public async Task<string> GetSideChainTokenAddressAsync()
        {
            var contractAddress = await _blockChainService.GetSideChainContractAddressAsync(ManagerToolkit.AelfSettings.MultiTokenContractAddress).ConfigureAwait(false);

            if (ManagerToolkit.AelfSettings.MultiTokenContractAddress != contractAddress)
            {
                _cache.Set("SideChainTokenAddress", contractAddress);
            }

            return contractAddress;
        }

        public string FetchSideChainTokenAddress()
        {
            var contractAddress = _cache.Get<string>("SideChainTokenAddress");

            if (string.IsNullOrEmpty(contractAddress))
            {
                contractAddress = ManagerToolkit.AelfSettings.MultiTokenContractAddress;
            }

            return contractAddress;
        }
    }
}
