using AElf;
using AElf.Client.Dto;
using AElf.Client.LatchBox.LockTokenVault;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models.Inputs;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class LockTokenVaultManager : ManagerBase, ILockTokenVaultManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletManager _walletManager;

        public LockTokenVaultManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IWalletManager walletManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _walletManager = walletManager;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.LockTokenVaultContractAddress;

        public async Task<TransactionResultDto> InitializeAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> AddLockAsync(AddLockInputModel model)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

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

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "AddLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimLockAsync(long lockId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new ClaimLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "ClaimLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> RevokeLockAsync(long lockId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new RevokeLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "RevokeLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new ClaimRefundInput()
            {
                TokenSymbol = tokenSymbol
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "ClaimRefund", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetLocksCountAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetLocksCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockTransactionOutput> GetLockTransactionAsync(long lockId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Int64Value() { Value = lockId };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetLockTransaction", @params);
            return GetLockTransactionOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetLocks", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByInitiatorAsync(string initiator)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetLocksByInitiator", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockReceiverListOutput> GetLocksForReceiverAsync(string receiver)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver).Value };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetLocksForReceiver", @params);
            return GetLockReceiverListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByAssetAsync(string tokenSymbol)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new StringValue() { Value = tokenSymbol };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetLocksByAsset", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetRefundListOutput> GetRefundsAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetRefunds", @params);
            return GetRefundListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockAssetCounterListOutput> GetAssetsCounterAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetAssetsCounter", @params);
            return GetLockAssetCounterListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }
    }
}
