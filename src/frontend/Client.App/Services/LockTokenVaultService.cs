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

        public async Task<GetLockListOutput> GetLocksByInitiatorAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<GetLockListOutput>(chain.LockVaultContractAddress, "GetLocksByInitiator", walletAddress);
        }
    }
}
