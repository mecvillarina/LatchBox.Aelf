using Client.Pages.Modals;
using Client.Shared.Dialogs;
using MudBlazor;

namespace Client.Shared.Components
{
    public partial class AppBarContent
    {
        public bool IsLoaded { get; set; }
        public bool IsPlatformTokenLoaded { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Network { get; set; }
        public string Node { get; set; }
        public string WalletAddress { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    IsAuthenticated = await AuthManager.IsAuthenticated();
                    Network = BlockchainManager.Network;
                    Node = BlockchainManager.Node;

                    if(IsAuthenticated)
                    {
                        var wallet = await WalletManager.GetWalletInformationAsync();
                        WalletAddress = wallet.Address;
                    }

                    IsLoaded = true;
                    StateHasChanged();
                });
            }
        }


        private void InvokeConnectWalletModal()
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };
            DialogService.Show<ConnectWalletModal>("Connect Wallet (JSON)", options);
        }

        private void InvokeDisconnectWalletDialog()
        {
            var parameters = new DialogParameters
            {
                {nameof(DisconnectWalletDialog.ContentText), "Are you sure you want to disconnect your wallet?"},
                {nameof(DisconnectWalletDialog.ButtonText), "Disconnect"},
                {nameof(DisconnectWalletDialog.Color), Color.Error},
            };

            DialogService.Show<DisconnectWalletDialog>("Logout", parameters);
        }
    }
}