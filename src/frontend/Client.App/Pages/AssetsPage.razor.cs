using Application.Common.Dtos;
using Client.App.Pages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages
{
    public partial class AssetsPage : IPageBase
    {
        public List<TokenBalanceInfoDto> TokenBalances { get; set; } = new();
        public bool IsLoaded { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchData();
            }
        }

        private async Task FetchData()
        {
            IsLoaded = false;
            StateHasChanged();

            var result = await TokenManager.GetTokenBalancesAsync("tdVV", "61bTPDbBwfB2abbB8oquerLyD3tmRyqjUk4YVN9QQvLJkN2mN");

            if (result.Succeeded)
            {
                TokenBalances = result.Data;
            }

            IsLoaded = true;
            StateHasChanged();
        }

        private async Task<(bool, List<string>)> ValidateSupportedChainAsync()
        {
            var chains = await ChainManager.FetchSupportedChainsAsync();
            var currentChain = await ChainManager.FetchCurrentChainAsync();
            var isSupported = chains.Any(x => x.ChainIdBase58 == currentChain && x.IsTokenCreationFeatureSupported);
            var supportedChains = chains.Where(x => x.IsTokenCreationFeatureSupported).Select(x => x.ChainIdBase58).ToList();
            return (isSupported, supportedChains);
        }

        private async Task OnCreateNewTokenAsync()
        {
            var result = await ValidateSupportedChainAsync();

            if (!result.Item1)
            {
                var message = "Token Creation feature is not supported on this chain.";

                if (result.Item2.Any())
                {
                    message = $"Token Creation feature is not supported on this chain. Currently, it is only supported on the following chains: <br><ul>{string.Join("",result.Item2.Select(x => $"<li>• {x}</li>").ToList())}</ul>";
                }

                _appDialogService.ShowError(message);
                return;
            }
        }
    }
}