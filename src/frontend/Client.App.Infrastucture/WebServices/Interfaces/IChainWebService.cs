using Application.Common.Dtos;
using Application.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices.Interfaces
{
    public interface IChainWebService : IWebService
    {
        Task<IResult<List<ChainInfoDto>>> GetAllSupportedChainsAsync();
    }
}