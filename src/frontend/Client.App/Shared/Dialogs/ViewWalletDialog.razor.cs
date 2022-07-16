using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Client.App.Shared.Dialogs
{
    public partial class ViewWalletDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        public string WalletAddress { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var walletAddress = await NightElfService.GetAddressAsync();
                WalletAddress = walletAddress;
                StateHasChanged();
            }
        }
        private async Task OnDisconnectAsync()
        {
            await NightElfService.LogoutAsync();
            NightElfExecutor.InvokeDisconnect();
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
