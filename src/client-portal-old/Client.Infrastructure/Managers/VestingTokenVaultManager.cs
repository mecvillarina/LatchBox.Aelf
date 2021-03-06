using AElf;
using AElf.Client.Dto;
using AElf.Client.LatchBox.VestingTokenVault;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models.Inputs;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class VestingTokenVaultManager : ManagerBase, IVestingTokenVaultManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletManager _walletManager;
        private readonly IBlockchainManager _blockchainManager;

        public VestingTokenVaultManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IWalletManager walletManager, IBlockchainManager blockchainManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _walletManager = walletManager;
            _blockchainManager = blockchainManager;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.VestingTokenVaultContractAddress;

        public async Task<TransactionResultDto> InitializeAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty { };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> AddVestingAsync(AddVestingInputModel model)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new AddVestingInput
            {
                TokenSymbol = model.TokenSymbol,
                TotalAmount = model.TotalAmount,
                IsRevocable = model.IsRevocable,
            };

            foreach (var period in model.Periods)
            {
                var periodParams = new AddVestingPeriodInput
                {
                    Name = period.Name,
                    TotalAmount = period.TotalAmount,
                    UnlockTime = Timestamp.FromDateTime(period.UnlockTime),
                };

                foreach (var receiver in period.Receivers)
                {
                    periodParams.Receivers.Add(new AddVestingReceiverInput()
                    {
                        Name = receiver.Name,
                        Address = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver.Address).Value },
                        Amount = receiver.Amount
                    });
                }

                @params.Periods.Add(periodParams);
            }

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "AddVesting", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimVestingAsync(long vestingId, long periodId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ClaimVestingInput { VestingId = vestingId, PeriodId = periodId };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "ClaimVesting", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> RevokeVestingAsync(long vestingId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new RevokeVestingInput { VestingId = vestingId };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "RevokeVesting", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ClaimRefundInput { TokenSymbol = tokenSymbol };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "ClaimRefund", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetVestingsCountAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();
            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetVestingsCount", @params, chainStatus);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetVestingTransactionOutput> GetVestingTransactionAsync(long vestingId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new Int64Value() { Value = vestingId };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetVestingTransaction", @params, chainStatus);
            return GetVestingTransactionOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetVestingListOutput> GetVestingsByInitiatorAsync(string initiator)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetVestingsByInitiator", @params, chainStatus);
            return GetVestingListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetVestingReceiverListOutput> GetVestingsForReceiverAsync(string receiver)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver).Value };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetVestingsForReceiver", @params, chainStatus);
            return GetVestingReceiverListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }


        public async Task<GetRefundListOutput> GetRefundsAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetRefunds", @params, chainStatus);
            return GetRefundListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

    }
}
