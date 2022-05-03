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
        public string RpcUrl { get; set; }

        private void InvokeConnectWalletModal()
        {
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