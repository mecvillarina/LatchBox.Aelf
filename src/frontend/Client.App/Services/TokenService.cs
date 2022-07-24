using Application.Common.Dtos;
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

        public async Task<TransactionResultDto> CreateTokenAsync(TokenCreateTokenInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.TokenContractAddress, "Create", input);
        }

        public async Task<TransactionResultDto> IssueTokenAsync(TokenIssueTokenInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.TokenContractAddress, "Issue", input);
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

        public async Task<TokenInfo> GetNativeTokenInfoAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<TokenInfo>(chain.TokenContractAddress, "GetNativeTokenInfo", "");
        }

        public async Task<TokenGetAllowanceOutput> GetAllowanceAsync(TokenGetAllowanceInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<TokenGetAllowanceOutput>(chain.TokenContractAddress, "GetAllowance", input);
        }

        public async Task<TransactionResultDto> ApproveAsync(TokenApproveInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.TokenContractAddress, "Approve", input);
        }
    }
}
