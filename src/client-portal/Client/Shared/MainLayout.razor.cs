using Client.Infrastructure.Extensions;
using MudBlazor;

namespace Client.Shared
{
    public partial class MainLayout : IAsyncDisposable
    {
        private bool IsAuthenticated { get; set; }
        private bool DrawerOpen { get; set; } = true;
        private MudTheme CurrentTheme { get; set; }
        public string MainChain { get; set; }
        public string SideChain { get; set; }
        public string MainChainNode { get; set; }
        public string SideChainNode { get; set; }
        public bool IsLoaded { get; set; }
        protected override void OnInitialized()
        {
            CurrentTheme = ClientPreferenceManager.GetCurrentTheme();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    IsAuthenticated = await AuthManager.IsAuthenticated();
                    await AppBreakpointService.InitAsync();

                    var mainChainStatus = BlockchainManager.FetchMainChainStatus();
                    if (mainChainStatus == null)
                    {
                        mainChainStatus = await BlockchainManager.GetMainChainStatusAsync();
                        if (mainChainStatus == null) return;
                        await BlockchainManager.GetMainChainTokenAddressAsync();
                    }

                    var sideChainStatus = BlockchainManager.FetchSideChainStatus();
                    if (sideChainStatus == null)
                    {
                        sideChainStatus = await BlockchainManager.GetSideChainStatusAsync();
                        if (sideChainStatus == null) return;
                        await BlockchainManager.GetSideChainTokenAddressAsync();
                    }

                    MainChain = $"Main {BlockchainManager.GetMainChainId().ToStringChainId()}";
                    SideChain = $"Side {BlockchainManager.GetSideChainId().ToStringChainId()}";
                    MainChainNode = BlockchainManager.MainChainNode;
                    SideChainNode = BlockchainManager.SideChainNode;
                    IsLoaded = true;
                    StateHasChanged();
                });
            }
        }

        public async ValueTask DisposeAsync()
        {
            await AppBreakpointService.DisposeAsync();
        }
    }
}