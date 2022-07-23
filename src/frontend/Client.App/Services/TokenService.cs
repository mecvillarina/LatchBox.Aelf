using Client.App.SmartContractDto;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class TokenService
    {
        private readonly NightElfService _nightElfService;
        private readonly ChainService _chainService;
        public TokenService(NightElfService nightElfService, ChainService chainService)
        {
            _nightElfService = nightElfService;
            _chainService = chainService;
        }

        public async Task<TokenGetBalanceOutput> GetBalanceAsync(TokenGetBalanceInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<TokenGetBalanceOutput>(chain.TokenContractAddress, "GetBalance", input);
        }

        public async Task<TokenInfo> GetTokenInfoAsync(TokenGetTokenInfoInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<TokenInfo>(chain.TokenContractAddress, "GetTokenInfo", input);
        }
    }
}
