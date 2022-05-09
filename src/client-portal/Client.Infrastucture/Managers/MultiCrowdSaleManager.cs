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
            var cred = await _walletManager.GetWalletCredentialsAsync();

            IMessage @params = new Empty { };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CreateAsync(CreateLaunchpadInputModel model)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

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

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Create", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CancelAsync(long crowdSaleId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new CancelInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Cancel", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> InvestAsync(long crowdSaleId, long amount)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new InvestInput()
            {
                CrowdSaleId = crowdSaleId,
                TokenAmount = amount
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Invest", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<CrowdSaleOutput> GetCrowdSaleAsync(long crowdSaleId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new GetCrowdSaleInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetCrowdSale", @params);
            return CrowdSaleOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleInvestorListOutput> GetCrowdSaleInvestorsAsync(long crowdSaleId)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new GetCrowdSaleInvestorsInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetCrowdSaleInvestors", @params);
            return CrowdSaleInvestorListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesByInitiatorAsync(string initiator)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetCrowdSalesByInitiator", @params);
            return CrowdSaleListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesAsync(bool isUpcoming, bool isOngoing)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new GetCrowdSalesInput()
            {
                IsUpcoming = isUpcoming,
                IsOngoing = isOngoing
            };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetCrowdSales", @params);
            return CrowdSaleListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }
    }
}
