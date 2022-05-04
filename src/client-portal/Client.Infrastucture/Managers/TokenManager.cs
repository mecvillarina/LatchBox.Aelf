using AElf;
using AElf.Client.MultiToken;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class TokenManager : ManagerBase, ITokenManager
    {
        private readonly IBlockChainService _blockChainService;

        public TokenManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
        }

        public async Task GetBalanceAsync(WalletInformation wallet, string password, string symbol)
        {
            var paramGetBalance = new GetBalanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(wallet.Address).Value }
            };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetBalance", paramGetBalance);
            var balance = GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task GetTokenInfoAsync(WalletInformation wallet, string password, string symbol)
        {
            var paramGetTokenInfo = new GetTokenInfoInput()
            {
                Symbol = symbol
            };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetTokenInfo", paramGetTokenInfo);
            var tokenInfo = TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task CreateTokenAsync(WalletInformation wallet, string password)
        {
            var paramCreateInput = new CreateInput
            {
                Symbol = "LATCH",
                TokenName = "LatchBoxToken",
                TotalSupply = 200_000_000_00000000,
                Decimals = 8,
                Issuer = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(wallet.Address).Value },
                IsBurnable = true,
                IssueChainId = 9992731
            };

            paramCreateInput.LockWhiteList.Add(new AElf.Client.Proto.Address() { Value = AElf.Types.Address.FromBase58(wallet.Address).Value });

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "Create", JsonConvert.SerializeObject(paramCreateInput));
            var result = await _blockChainService.CheckTransactionResultAsync(txId);

        }
    }
}
