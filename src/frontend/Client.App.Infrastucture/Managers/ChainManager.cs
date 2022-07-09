using Application.Common.Dtos;
using Application.Common.Models;
using Client.App.Infrastructure.Managers.Interfaces;
using Client.App.Infrastructure.WebServices.Interfaces;
using Client.Infrastructure.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers
{
    public class ChainManager : ManagerBase, IChainManager
    {
        private readonly IChainWebService _chainWebService;
        public ChainManager(IManagerToolkit managerToolkit, IChainWebService chainWebService) : base(managerToolkit)
        {
            _chainWebService = chainWebService;
        }

        public async Task<IResult<List<ChainInfoDto>>> GetAllSupportedChainsAsync()
        {
            var response = await _chainWebService.GetAllSupportedChainsAsync();
            if (response.Succeeded)
            {
                await ManagerToolkit.SaveDataAsync(StorageConstants.Local.SupportedChains, response.Data);
            }

            return response;
        }

        public async Task<List<ChainInfoDto>> FetchSupportedChainsAsync()
        {
            var data = await ManagerToolkit.GetDataAsync<List<ChainInfoDto>>(StorageConstants.Local.SupportedChains);
            data ??= new List<ChainInfoDto>();
            return data;
        }

        public async Task<string> FetchCurrentChainAsync()
        {
            var data = await ManagerToolkit.GetDataAsync<string>(StorageConstants.Local.CurrentChain);
            return data;
        }

        public async Task SetCurrentChainAsync(string chainInfo)
        {
            await ManagerToolkit.SaveDataAsync(StorageConstants.Local.CurrentChain, chainInfo);
        }
    }
}
