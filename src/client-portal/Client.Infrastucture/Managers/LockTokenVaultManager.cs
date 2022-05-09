using AElf;
using AElf.Client.Dto;
using AElf.Client.LatchBox.LockTokenVault;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Models.Inputs;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class LockTokenVaultManager : ManagerBase, ILockTokenVaultManager
    {
        private readonly IBlockChainService _blockChainService;

        public LockTokenVaultManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.LockTokenVaultContractAddress;

        public async Task<TransactionResultDto> InitializeAsync(WalletInformation wallet, string password)
        {
            var @params = new InitializeInput { };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> AddLockAsync(WalletInformation wallet, string password, AddLockInputModel model)
        {
            var @params = new AddLockInput()
            {
                TokenSymbol = model.TokenSymbol,
                TotalAmount = model.TotalAmount,
                UnlockTime = Timestamp.FromDateTime(model.UnlockTime),
                IsRevocable = model.IsRevocable,
            };

            foreach (var receiver in model.Receivers)
            {
                @params.Receivers.Add(new AddLockReceiverInput()
                {
                    Receiver = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver.ReceiverAddress).Value },
                    Amount = receiver.Amount
                });
            }

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "AddLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimLockAsync(WalletInformation wallet, string password, long lockId)
        {
            var @params = new ClaimLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "ClaimLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> RevokeLockAsync(WalletInformation wallet, string password, long lockId)
        {
            var @params = new RevokeLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "RevokeLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(WalletInformation wallet, string password, string tokenSymbol)
        {
            var @params = new ClaimRefundInput()
            {
                TokenSymbol = tokenSymbol
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "ClaimRefund", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetLocksCountAsync(WalletInformation wallet, string password)
        {
            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetLocksCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockTransactionOutput> GetLockTransactionAsync(WalletInformation wallet, string password, long lockId)
        {
            var @params = new Int64Value() { Value = lockId };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetLockTransaction", @params);
            return GetLockTransactionOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksAsync(WalletInformation wallet, string password)
        {
            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetLocks", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByInitiatorAsync(WalletInformation wallet, string password, string initiator)
        {
            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetLocksByInitiator", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockReceiverListOutput> GetLocksForReceiverAsync(WalletInformation wallet, string password, string receiver)
        {
            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver).Value };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetLocksForReceiver", @params);
            return GetLockReceiverListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByAssetAsync(WalletInformation wallet, string password, string tokenSymbol)
        {
            var @params = new StringValue() { Value = tokenSymbol };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetLocksByAsset", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetRefundListOutput> GetRefundsAsync(WalletInformation wallet, string password)
        {
            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetRefunds", @params);
            return GetRefundListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockAssetCounterListOutput> GetAssetsCounterAsync(WalletInformation wallet, string password)
        {
            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetAssetsCounter", @params);
            return GetLockAssetCounterListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }
    }
}
