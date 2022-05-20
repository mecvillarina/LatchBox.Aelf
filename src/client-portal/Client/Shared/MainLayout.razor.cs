using MudBlazor;

namespace Client.Shared
{
    public partial class MainLayout : IAsyncDisposable
    {
        private bool IsAuthenticated { get; set; }
        private bool DrawerOpen { get; set; } = true;
        private MudTheme CurrentTheme { get; set; }
        public string Network { get; set; }
        public string Node { get; set; }
        public int ChainId { get; set; }

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
                    ChainId = await BlockchainManager.GetChainIdAsync();
                    Network = $"{BlockchainManager.Network} ({ChainId})";
                    Node = BlockchainManager.Node;
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