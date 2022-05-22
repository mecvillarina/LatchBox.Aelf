using AElf;
using AElf.Client.Dto;
using AElf.Client.LatchBox.MultiCrowdSale;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models.Inputs;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class MultiCrowdSaleManager : ManagerBase, IMultiCrowdSaleManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletManager _walletManager;

        public MultiCrowdSaleManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IWalletManager walletManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _walletManager = walletManager;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.MultiCrowdSaleContractAddress;

        public async Task<TransactionResultDto> InitializeAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            IMessage @params = new Empty { };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CreateAsync(CreateLaunchpadInputModel model)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new CreateInput()
            {
                Name = model.Name,
                TokenSymbol = model.TokenSymbol,
                SoftCapNativeTokenAmount = model.SoftCapNativeTokenAmount,
                HardCapNativeTokenAmount = model.HardCapNativeTokenAmount,
                NativeTokenPurchaseLimitPerBuyerAddress = model.NativeTokenPurchaseLimitPerBuyerAddress,
                TokenAmountPerNativeToken = model.TokenAmountPerNativeToken,
                SaleStartDate = Timestamp.FromDateTime(model.SaleStartDate),
                SaleEndDate = Timestamp.FromDateTime(model.SaleEndDate),
                LockUntilDurationInMinutes = model.LockUntilDurationInMinutes
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Create", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CancelAsync(long crowdSaleId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new CancelInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Cancel", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> InvestAsync(long crowdSaleId, long amount)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new InvestInput()
            {
                CrowdSaleId = crowdSaleId,
                TokenAmount = amount
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Invest", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CompleteAsync(long crowdSaleId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new CompleteInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "Complete", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> UpdateLockInfoAsync(long crowdSaleId, long lockId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new UpdateLockInfoInput()
            {
                CrowdSaleId = crowdSaleId,
                LockId = lockId,
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, ContactAddress, "UpdateLockInfo", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<Int64Value> GetCrowdSaleCountAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetCrowdSaleCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<Int64Value> GetUpcomingCrowdSaleCountAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetUpcomingCrowdSaleCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<Int64Value> GetOngoingCrowdSaleCountAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new Empty();

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetOngoingCrowdSaleCount", @params);
            return Int64Value.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleOutput> GetCrowdSaleAsync(long crowdSaleId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new GetCrowdSaleInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetCrowdSale", @params);
            return CrowdSaleOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesByInitiatorAsync(string initiator)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetCrowdSalesByInitiator", @params);
            return CrowdSaleListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleByInvestorListOuput> GetCrowdSalesByInvestorAsync(string investor)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(investor).Value };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetCrowdSalesByInvestor", @params);
            return CrowdSaleByInvestorListOuput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesAsync(bool isUpcoming, bool isOngoing)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new GetCrowdSalesInput()
            {
                IsUpcoming = isUpcoming,
                IsOngoing = isOngoing
            };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetCrowdSales", @params);
            return CrowdSaleListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleInvestmentListOutput> GetCrowdSaleInvestments(long crowdSaleId)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new GetCrowdSaleInvestorsInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, ContactAddress, "GetCrowdSaleInvestments", @params);
            return CrowdSaleInvestmentListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

    }
}
