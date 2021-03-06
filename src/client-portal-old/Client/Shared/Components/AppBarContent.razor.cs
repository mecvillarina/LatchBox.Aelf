using Client.Infrastructure.Managers.Interfaces;
using Client.Pages.Modals;
using Client.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Shared.Components
{
    public partial class AppBarContent
    {
        [Inject] public IFaucetManager FaucetManager { get; set; }

        public bool IsLoaded { get; set; }
        public bool IsPlatformTokenLoaded { get; set; }
        public bool IsAuthenticated { get; set; }
        public string WalletAddress { get; set; }
        public bool IsClaimingElf { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsAuthenticated = await AuthManager.IsAuthenticated();

                if (IsAuthenticated)
                {
                    WalletAddress = await WalletManager.GetWalletAddressAsync();
                }

                IsLoaded = true;
                StateHasChanged();
            }
        }

        private void InvokeConnectWalletModal()
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };
            DialogService.Show<ConnectWalletModal>("Connect Wallet KeyStore (json)", options);
        }

        private async Task InvokeClaimELFAsync()
        {
            IsClaimingElf = true;

            var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

            if (authenticated)
            {
                try
                {
                    var result = await FaucetManager.TakeAsync("ELF", 100_00000000);

                    if (!string.IsNullOrEmpty(result.Error))
                    {
                        AppDialogService.ShowError(result.Error);
                    }
                    else
                    {
                        AppDialogService.ShowSuccess("[Main AELF Chain] Claim ELF Success.");
                    }
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                }
            }

            IsClaimingElf = false;
        }

        private void InvokeDisconnectWalletDialog()
        {
            var options = new DialogOptions()
            {
                CloseButton = true
            };

            DialogService.Show<ViewWalletDialog>("Your Wallet Address", options);
        }
    }
}