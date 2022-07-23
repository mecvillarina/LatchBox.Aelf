using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class LockTokenVaultService
    {
        private readonly NightElfService _nightElfService;
        private readonly ChainService _chainService;
        public LockTokenVaultService(NightElfService nightElfService, ChainService chainService)
        {
            _nightElfService = nightElfService;
            _chainService = chainService;
        }

        public async Task<LockGetLockListOutput> GetLocksByInitiatorAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<LockGetLockListOutput>(chain.LockVaultContractAddress, "GetLocksByInitiator", walletAddress);
        }

        public async Task<LockGetLockReceiverListOutput> GetLocksForReceiverAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<LockGetLockReceiverListOutput>(chain.LockVaultContractAddress, "GetLocksForReceiver", walletAddress);
        }
    }
}
