using Application.Common.Dtos;
using Application.Common.Models;
using Client.App.Infrastructure.Routes;
using Client.App.Infrastructure.WebServices.Base;
using Client.App.Infrastructure.WebServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices
{
    public class TokenWebService : WebServiceBase, ITokenWebService
    {
        public TokenWebService(AppHttpClient appHttpClient) : base(appHttpClient)
        {
        }

        public Task<IResult<List<TokenDto>>> GetAllTokensAsync(string chainIdBase58) => GetAsync<List<TokenDto>>(string.Format(TokenEndpoint.GetAllTokens, chainIdBase58));
        public Task<IResult<List<TokenBalanceInfoDto>>> GetTokenBalancesAsync(string chainIdBase58, string address) => GetAsync<List<TokenBalanceInfoDto>>(string.Format(TokenEndpoint.GetTokenBalances, chainIdBase58, address));

    }
}
