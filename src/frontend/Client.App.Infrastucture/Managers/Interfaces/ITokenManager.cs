using Application.Common.Dtos;
using Application.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers.Interfaces
{
    public interface ITokenManager : IManager
    {
        Task<IResult<List<TokenDto>>> GetAllTokensAsync(string chainIdBase58);
        Task<IResult<List<TokenBalanceInfoDto>>> GetTokenBalancesAsync(string chainIdBase58, string address);
    }
}