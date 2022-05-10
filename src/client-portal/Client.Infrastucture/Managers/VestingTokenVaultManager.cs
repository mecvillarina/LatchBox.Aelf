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

        public VestingTokenVaultManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IWalletManager walletManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _walletManager = walletManager;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.VestingTokenVaultContractAddress;

        public async Task<TransactionResultDto> InitializeAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty { };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> AddVestingAsync(AddVestingInputModel model)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

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

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "AddVesting", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimVestingAsync(long vestingId, long periodId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new ClaimVestingInput { VestingId = vestingId, PeriodId = periodId };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "ClaimVesting", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> RevokeVestingAsync(long vestingId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new RevokeVestingInput { VestingId = vestingId };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "RevokeVesting", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new ClaimRefundInput { TokenSymbol = tokenSymbol };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "ClaimRefund", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetVestingsCountAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetVestingsCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetVestingTransactionOutput> GetVestingTransactionAsync(long vestingId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Int64Value() { Value = vestingId };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetVestingTransaction", @params);
            return GetVestingTransactionOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetVestingListOutput> GetVestingsByInitiatorAsync(string initiator)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetVestingsByInitiator", @params);
            return GetVestingListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetVestingReceiverListOutput> GetVestingsForReceiverAsync(string receiver)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver).Value };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetVestingsForReceiver", @params);
            return GetVestingReceiverListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }


        public async Task<GetRefundListOutput> GetRefundsAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetRefunds", @params);
            return GetRefundListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

    }
}
