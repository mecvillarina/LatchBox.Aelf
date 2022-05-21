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
                    MainChain = $"Main {(await BlockchainManager.GetMainChainIdAsync()).ToStringChainId()}";
                    SideChain = $"Side {(await BlockchainManager.GetSideChainIdAsync()).ToStringChainId()}";
                    MainChainNode = BlockchainManager.MainChainNode;
                    SideChainNode = BlockchainManager.SideChainNode;
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