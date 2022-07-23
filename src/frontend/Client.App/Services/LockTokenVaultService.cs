﻿using Application.Common.Dtos;
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
