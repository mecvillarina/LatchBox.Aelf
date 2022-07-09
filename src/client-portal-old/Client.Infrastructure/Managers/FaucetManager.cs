using AElf.Client.Dto;
using AElf.Client.Faucet;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class FaucetManager : ManagerBase, IFaucetManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletManager _walletManager;

        public FaucetManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, IWalletManager walletManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _walletManager = walletManager;
        }

        public async Task<TransactionResultDto> TakeAsync(string symbol, long amount)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new TakeInput()
            {
                Symbol = symbol,
                Amount = amount
            };

            var tx = await _blockChainService.SendMainChainTransactionAsync(keyPair, ManagerToolkit.AelfSettings.FaucetContractAddress, "Take", @params);
            return await _blockChainService.CheckMainChainTransactionResultAsync(tx.Item1);
        }
    }
}
