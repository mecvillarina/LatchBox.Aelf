using Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Shared.Dialogs
{
    public partial class ViewWalletDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public string ContentText { get; set; }

        [Parameter] public string ButtonText { get; set; }

        [Parameter] public Color Color { get; set; }

        public string MainChainWalletAddress { get; set; }
        public string MainChainWalletAddressLink { get; set; }
        public string SideChainWalletAddress { get; set; }
        public string SideChainWalletAddressLink { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var walletAddress = await WalletManager.GetWalletAddressAsync();
                var mainChain = BlockchainManager.GetMainChainId().ToChainName();
                var sideChain = BlockchainManager.GetSideChainId().ToChainName();
                MainChainWalletAddress = $"ELF_{walletAddress}_{mainChain}";
                SideChainWalletAddress = $"ELF_{walletAddress}_{sideChain}";
                MainChainWalletAddressLink = $"{BlockchainManager.MainChainExplorer}/address/{walletAddress}";
                SideChainWalletAddressLink = $"{BlockchainManager.SideChainExplorer}/address/{walletAddress}";
                StateHasChanged();
            }
        }

        private async Task Submit()
        {
            await AuthManager.DisconnectWalletAsync();
            NavigationManager.NavigateTo("/", true);
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
