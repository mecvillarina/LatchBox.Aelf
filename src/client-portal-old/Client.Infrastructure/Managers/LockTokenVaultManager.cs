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
        private readonly IBlockchainManager _blockchainManager;

        public LockTokenVaultManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IWalletManager walletManager, IBlockchainManager blockchainManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _walletManager = walletManager;
            _blockchainManager = blockchainManager;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.LockTokenVaultContractAddress;

        public async Task<TransactionResultDto> InitializeAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
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

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "AddLock", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimLockAsync(long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ClaimLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "ClaimLock", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> RevokeLockAsync(long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new RevokeLockInput()
            {
                LockId = lockId,
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "RevokeLock", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ClaimRefundInput()
            {
                TokenSymbol = tokenSymbol
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "ClaimRefund", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetLocksCountAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();
            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetLocksCount", @params, chainStatus);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockTransactionOutput> GetLockTransactionAsync(long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new Int64Value() { Value = lockId };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetLockTransaction", @params, chainStatus);
            return GetLockTransactionOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetLocks", @params, chainStatus);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByInitiatorAsync(string initiator)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetLocksByInitiator", @params, chainStatus);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockReceiverListOutput> GetLocksForReceiverAsync(string receiver)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(receiver).Value };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetLocksForReceiver", @params, chainStatus);
            return GetLockReceiverListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockListOutput> GetLocksByAssetAsync(string tokenSymbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new StringValue() { Value = tokenSymbol };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetLocksByAsset", @params, chainStatus);
            return GetLockListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetRefundListOutput> GetRefundsAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetRefunds", @params, chainStatus);
            return GetRefundListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetLockAssetCounterListOutput> GetAssetsCounterAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetAssetsCounter", @params, chainStatus);
            return GetLockAssetCounterListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }
    }
}
