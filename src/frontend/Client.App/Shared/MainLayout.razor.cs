using Client.App.Pages.Base;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Shared
{
    public partial class MainLayout : IPageBase, IAsyncDisposable
    {
        private bool DrawerOpen { get; set; } = true;
        private MudTheme CurrentTheme { get; set; }
        public bool IsLoaded { get; set; }

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
                    var data = await ChainManager.FetchSupportedChainsAsync();
                    IsLoaded = false;
                    if (!data.Any())
                    {
                        var chainsResult = await _exceptionHandler.HandlerRequestTaskAsync(() => ChainManager.GetAllSupportedChainsAsync());

                        if (chainsResult.Succeeded)
                        {
                            if (chainsResult.Data.Any())
                            {
                                await ChainManager.SetCurrentChainAsync(chainsResult.Data.First().ChainIdBase58);
                                IsLoaded = true;
                            }
                        }
                    }
                    else
                    {
                        var currentChain = await ChainManager.FetchCurrentChainAsync();
                        if(currentChain == null || !data.Any(x => x.ChainIdBase58 == currentChain))
                        {
                            await ChainManager.SetCurrentChainAsync(data.First().ChainIdBase58);
                        }
                        IsLoaded = true;
                    }

                    StateHasChanged();
                }
            }
            catch { }
        }

        public async ValueTask DisposeAsync()
        {
            await AppBreakpointService.DisposeAsync();
        }
    }
}