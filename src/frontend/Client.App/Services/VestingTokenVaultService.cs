using Application.Common.Dtos;
using Client.App.SmartContractDto.LockTokenVault;
using Client.App.SmartContractDto.VestingTokenVault;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class VestingTokenVaultService
    {
        private readonly NightElfService _nightElfService;
        private readonly ChainService _chainService;

        public VestingTokenVaultService(NightElfService nightElfService, ChainService chainService)
        {
            _nightElfService = nightElfService;
            _chainService = chainService;
        }

        public async Task<VestingListOutput> GetVestingsByInitiatorAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<VestingListOutput>(chain.VestingVaultContractAddress, "GetVestingsByInitiator", walletAddress);
        }

        public async Task<VestingReceiverListOutput> GetVestingsForReceiverAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<VestingReceiverListOutput>(chain.VestingVaultContractAddress, "GetVestingsForReceiver", walletAddress);
        }

        public async Task<VestingRefundListOutput> GetRefundsAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<VestingRefundListOutput>(chain.VestingVaultContractAddress, "GetRefunds", "");
        }

        public async Task<TransactionResultDto> AddVestingAsync(VestingAddVestingInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.VestingVaultContractAddress, "AddVesting", input);
        }

    }
}
