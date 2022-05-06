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

        public async Task<TransactionResultDto> CreateAsync(WalletInformation wallet, string password, CreateCrowdSaleInputModel model)
        {
            CreateInput @params = new CreateInput()
            {
                Name = model.Name,
                TokenSymbol = model.TokenSymbol,
                SoftCapNativeTokenAmount = model.SoftCapNativeTokenAmount,
                HardCapNativeTokenAmount = model.HardCapNativeTokenAmount,
                NativeTokenPurchaseLimitPerBuyerAddress = model.NativeTokenPurchaseLimitPerBuyerAddress,
                TokenAmountPerNativeToken = model.TokenAmountPerNativeToken,
                SaleEndDate = Timestamp.FromDateTime(model.SaleEndDate),
                LockUntilDurationInMinutes = model.LockUntilDurationInMinutes
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ContactAddress, "Create", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }
    }
}
