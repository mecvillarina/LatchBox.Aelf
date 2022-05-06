using AElf.Client.Dto;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class FaucetManager : ManagerBase, IFaucetManager
    {
        private readonly IBlockChainService _blockChainService;
        public FaucetManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
        }

        public async Task<TransactionResultDto> TakeAsync(WalletInformation wallet, string password, string symbol, long amount)
        {
            var @params = new JObject();
            @params["symbol"] = symbol;
            @params["amount"] = amount;

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.FaucetContractAddress, "Take", JsonConvert.SerializeObject(@params));
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }
    }
}
