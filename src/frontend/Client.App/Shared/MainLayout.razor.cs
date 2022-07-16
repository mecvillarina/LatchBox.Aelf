using Client.App.Models;
using Client.App.Pages.Base;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Shared
{
    public partial class MainLayout : IPageBase, IAsyncDisposable
    {
        private bool DrawerOpen { get; set; } = true;
        private MudTheme CurrentTheme { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsNavMenuPanelExpanded { get; set; }

        public string CurrentChainIdBase58 { get; set; }
        public string CurrentChainDisplay => $"Chain ({CurrentChainIdBase58})";

        public List<SupportedChainModel> SupportedChains { get; set; } = new();

        protected override void OnInitialized()
        {
            CurrentTheme = _clientPreferenceManager.GetCurrentTheme();
            FetchDataExecutor.StartExecuting();
        }

        protected async override Task OnInitializedAsync()
        {
            await AppBreakpointService.InitAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {

            try
            {
                if (firstRender)
                {
                    await FetchChainDataAsync();
                }
            }
            catch { }
        }

        private async Task FetchChainDataAsync()
        {
            IsLoaded = await ChainService.FetchChainDataAsync();

            if (IsLoaded)
            {
                var supportedChains = await ChainService.FetchSupportedChainsAsync();
                SupportedChains = supportedChains.Select(x => new SupportedChainModel() { ChainType = x.ChainType, ChainIdBase58 = x.ChainIdBase58 }).ToList();
                CurrentChainIdBase58 = await ChainService.FetchCurrentChainAsync();
            }

            StateHasChanged();
        }

        private async Task SetChainAsync(string chainIdBase58)
        {
            Console.WriteLine(chainIdBase58);

            var currentChainIdBase58 = await ChainService.FetchCurrentChainAsync();
            if (chainIdBase58 == currentChainIdBase58) return;

            var data = await ChainService.FetchSupportedChainsAsync();
            if (data.Any(x => x.ChainIdBase58 == chainIdBase58))
            {
                await ChainService.SetCurrentChainAsync(chainIdBase58);
                _navigationManager.NavigateTo(_navigationManager.Uri, true);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await AppBreakpointService.DisposeAsync();
        }
    }
}