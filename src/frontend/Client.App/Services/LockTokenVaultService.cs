using Application.Common.Dtos;
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


        public async Task<LockGetLockTransactionOutput> GetLockTransactionAsync(long lockId)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();

            var payload = new GenericInput<long>() { Value = lockId };

            return await _nightElfService.CallTx<LockGetLockTransactionOutput>(chain.LockVaultContractAddress, "GetLockTransaction", payload);
        }

        public async Task<LockListOutput> GetLocksByInitiatorAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();

            return await _nightElfService.CallTx<LockListOutput>(chain.LockVaultContractAddress, "GetLocksByInitiator", walletAddress);
        }

        public async Task<LockReceiverListOutput> GetLocksForReceiverAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<LockReceiverListOutput>(chain.LockVaultContractAddress, "GetLocksForReceiver", walletAddress);
        }

        public async Task<LockRefundListOutput> GetRefundsAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<LockRefundListOutput>(chain.LockVaultContractAddress, "GetRefunds", "");
        }

        public async Task<TransactionResultDto> AddLockAsync(LockAddLockInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LockVaultContractAddress, "AddLock", input);
        }

        public async Task<TransactionResultDto> ClaimLockAsync(LockClaimInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LockVaultContractAddress, "ClaimLock", input);
        }

        public async Task<TransactionResultDto> RevokeLockAsync(LockRevokeInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LockVaultContractAddress, "RevokeLock", input);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(LockClaimRefundInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LockVaultContractAddress, "ClaimRefund", input);
        }
    }
}
