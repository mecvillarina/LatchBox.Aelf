using Application.Common.Dtos;
using Application.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers.Interfaces
{
    public interface IChainManager : IManager
    {
        Task<IResult<List<ChainInfoDto>>> GetAllSupportedChainsAsync();
        Task<List<ChainInfoDto>> FetchSupportedChainsAsync();
        Task<string> FetchCurrentChainAsync();
        Task SetCurrentChainAsync(string chainInfo);
    }
}