using AElf;
using AElf.Client.Dto;
using AElf.Client.LatchBox.MultiCrowdSale;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
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

        public MultiCrowdSaleManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.MultiCrowdSaleContractAddress;

        public async Task<TransactionResultDto> InitializeAsync(WalletInformation wallet, string password)
        {
            IMessage @params = new Empty { };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "Initialize", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CreateAsync(WalletInformation wallet, string password, CreateLaunchpadInputModel model)
        {
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

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "Create", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> CancelAsync(WalletInformation wallet, string password, long crowdSaleId)
        {
            var @params = new CancelInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "Cancel", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> InvestAsync(WalletInformation wallet, string password, long crowdSaleId, long amount)
        {
            var @params = new InvestInput()
            {
                CrowdSaleId = crowdSaleId,
                TokenAmount = amount
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "Invest", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<CrowdSaleOutput> GetCrowdSaleAsync(WalletInformation wallet, string password, long crowdSaleId)
        {
            var @params = new GetCrowdSaleInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetCrowdSale", @params);
            return CrowdSaleOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleInvestorListOutput> GetCrowdSaleInvestorsAsync(WalletInformation wallet, string password, long crowdSaleId)
        {
            var @params = new GetCrowdSaleInvestorsInput()
            {
                CrowdSaleId = crowdSaleId
            };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetCrowdSaleInvestors", @params);
            return CrowdSaleInvestorListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesByInitiatorAsync(WalletInformation wallet, string password, string initiator)
        {
            var @params = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(initiator).Value };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetCrowdSalesByInitiator", @params);
            return CrowdSaleListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesAsync(WalletInformation wallet, string password, bool isUpcoming, bool isOngoing)
        {
            var @params = new GetCrowdSalesInput()
            {
                IsUpcoming = isUpcoming,
                IsOngoing = isOngoing
            };

            var result = await _blockChainService.CallTransactionAsync(wallet, password, ContactAddress, "GetCrowdSales", @params);
            return CrowdSaleListOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }
    }
}
