using Application.Common.Dtos;
using Application.Common.Models;
using Client.App.Infrastructure.Managers.Interfaces;
using Client.App.Infrastructure.WebServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers
{
    public class TokenManager : ManagerBase, ITokenManager
    {
        private readonly ITokenWebService _tokenWebService;
        public TokenManager(IManagerToolkit managerToolkit, ITokenWebService tokenWebService) : base(managerToolkit)
        {
            _tokenWebService = tokenWebService;
        }

        public async Task<IResult<List<TokenDto>>> GetAllTokensAsync(string chainIdBase58)
        {
            var response = await _tokenWebService.GetAllTokensAsync(chainIdBase58);
            return response;
        }

        public async Task<IResult<List<TokenBalanceInfoDto>>> GetTokenBalancesAsync(string chainIdBase58, string address)
        {
            var response = await _tokenWebService.GetTokenBalancesAsync(chainIdBase58, address);
            return response;
        }
    }
}
