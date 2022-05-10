using AElf.Client.Dto;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new JObject();
            @params["symbol"] = symbol;
            @params["amount"] = amount;

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ManagerToolkit.AelfSettings.FaucetContractAddress, "Take", JsonConvert.SerializeObject(@params));
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }
    }
}
