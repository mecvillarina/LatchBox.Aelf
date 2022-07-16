using Application.Common.Dtos;
using Client.App.Pages.Assets.Modals;
using Client.App.Pages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages
{
    public partial class AssetsPage : IPageBase, IDisposable
    {
        public List<TokenBalanceInfoDto> TokenBalances { get; set; } = new();
        public bool IsLoaded { get; set; }

        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;

            if (!NightElfExecutor.IsConnected)
            {
                AppDialogService.ShowError("Connect wallet first.");
                NavigationManager.NavigateTo("/");
            }
        }

        private void HandleNightElfExecutorDisconnected(object source, EventArgs e)
        {
            NavigationManager.NavigateTo("/");
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            try
            {
                var currentChainBase58 = await ChainService.FetchCurrentChainAsync();
                var walletAddress = await NightElfService.GetAddressAsync();
                var result = await TokenManager.GetTokenBalancesAsync(currentChainBase58, walletAddress);

                if (result.Succeeded)
                {
                    TokenBalances = result.Data;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            IsLoaded = true;
            StateHasChanged();
        }

        private async Task<(bool, List<string>)> ValidateSupportedChainAsync()
        {
            var chains = await ChainService.FetchSupportedChainsAsync();
            var currentChain = await ChainService.FetchCurrentChainAsync();
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
                    message = $"Token Creation feature is not supported on this chain. Currently, it is only supported on the following chains: <br><ul>{string.Join("", result.Item2.Select(x => $"<li>• {x}</li>").ToList())}</ul>";
                }

                AppDialogService.ShowError(message);
                return;
            }

            var dialog = DialogService.Show<CreateTokenModal>($"Create New Token");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
            }
        }

        public void Dispose()
        {
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}