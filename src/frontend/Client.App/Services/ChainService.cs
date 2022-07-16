using Application.Common.Dtos;
using Application.Common.Models;
using Client.App.Infrastructure.Managers.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class ChainService
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IChainManager _chainManager;

        public ChainService(ExceptionHandler exceptionHandler, IChainManager chainManager)
        {
            _exceptionHandler = exceptionHandler;
            _chainManager = chainManager;
        }

        public Task<List<ChainInfoDto>> FetchSupportedChainsAsync() => _chainManager.FetchSupportedChainsAsync();
        public Task<string> FetchCurrentChainAsync() => _chainManager.FetchCurrentChainAsync();
        public Task SetCurrentChainAsync(string chainInfo) => _chainManager.SetCurrentChainAsync(chainInfo);
        public async Task<IResult<List<ChainInfoDto>>> GetAllSupportedChainsAsync()
        {
            return await _exceptionHandler.HandlerRequestTaskAsync(() => _chainManager.GetAllSupportedChainsAsync());
        }

        public async Task<bool> FetchChainDataAsync()
        {
            var isLoaded = false;
            var data = await _chainManager.FetchSupportedChainsAsync();
            if (!data.Any())
            {
                var chainsResult = await _exceptionHandler.HandlerRequestTaskAsync(() => _chainManager.GetAllSupportedChainsAsync());

                if (chainsResult.Succeeded)
                {
                    if (chainsResult.Data.Any())
                    {
                        await _chainManager.SetCurrentChainAsync(chainsResult.Data.First().ChainIdBase58);
                        isLoaded = true;
                    }
                }
            }
            else
            {
                var currentChain = await _chainManager.FetchCurrentChainAsync();
                if (currentChain == null || !data.Any(x => x.ChainIdBase58 == currentChain))
                {
                    await _chainManager.SetCurrentChainAsync(data.First().ChainIdBase58);
                }
                isLoaded = true;
            }

            return isLoaded;
        }
    }
}
