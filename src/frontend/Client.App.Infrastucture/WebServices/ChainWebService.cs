using Application.Common.Dtos;
using Application.Common.Models;
using Client.App.Infrastructure.Routes;
using Client.App.Infrastructure.WebServices.Base;
using Client.App.Infrastructure.WebServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices
{
    public class ChainWebService : WebServiceBase, IChainWebService
    {
        public ChainWebService(AppHttpClient appHttpClient) : base(appHttpClient)
        {
        }

        public Task<IResult<List<ChainInfoDto>>> GetAllSupportedChainsAsync() => GetAsync<List<ChainInfoDto>>(ChainEndpoints.GetAllSupportedChains);

    }
}
