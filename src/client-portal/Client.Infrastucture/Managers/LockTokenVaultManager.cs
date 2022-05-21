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
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var txId = await _blockChainService.SendTransactionAsync(keyPair, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> AddLockAsync(AddLockInputModel model)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new AddLockInput()
            {
                TokenSymbol = model.TokenSymbol,
                TotalAmount = model.TotalAmount,
                UnlockTime = Timestamp.FromDateTime(model.UnlockTime),
                IsRevocable = model.IsRevocable,
                Remarks = model.Remarks
            };

            foreach (var receiver in model.Receivers)
            {
                @params.Receivers.Add(new AddLockReceiverInput()
                {
                    Receiver = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver.ReceiverAddress).Value },
                    Amount = receiver.Amount
                });
            }

            var txId = await _blockChainService.SendTransactionAsync(keyPair, ContactAddress, "AddLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimLockAsync(long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ClaimLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendTransactionAsync(keyPair, ContactAddress, "ClaimLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> RevokeLockAsync(long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new RevokeLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendTransactionAsync(keyPair, ContactAddress, "RevokeLock", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ClaimRefundInput()
            {
                TokenSymbol = tokenSymbol
            };

            var txId = await _blockChainService.SendTransactionAsync(keyPair, ContactAddress, "ClaimRefund", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetLocksCountAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetLocksCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockTransactionOutput> GetLockTransactionAsync(long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Int64Value() { Value = lockId };

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetLockTransaction", @params);
            return GetLockTransactionOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetLocks", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByInitiatorAsync(string initiator)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetLocksByInitiator", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockReceiverListOutput> GetLocksForReceiverAsync(string receiver)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver).Value };

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetLocksForReceiver", @params);
            return GetLockReceiverListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByAssetAsync(string tokenSymbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new StringValue() { Value = tokenSymbol };

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetLocksByAsset", @params);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetRefundListOutput> GetRefundsAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetRefunds", @params);
            return GetRefundListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockAssetCounterListOutput> GetAssetsCounterAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetAssetsCounter", @params);
            return GetLockAssetCounterListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }
    }
}
