using Application.Common.Dtos;
using Application.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices.Interfaces
{
    public interface ITokenWebService : IWebService
    {
        Task<IResult<List<TokenDto>>> GetAllTokensAsync(string chainIdBase58);
        Task<IResult<List<TokenBalanceInfoDto>>> GetTokenBalancesAsync(string chainIdBase58, string address);
    }
}