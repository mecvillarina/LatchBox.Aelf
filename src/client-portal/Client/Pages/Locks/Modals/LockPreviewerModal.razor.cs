using Client.Infrastructure.Models;
using Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks.Modals
{
    public partial class LockPreviewerModal
    {
        [Parameter] public long LockId { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsLoaded { get; set; }
        public LockTransactionModel Model { get; set; }
        public string ShareLink { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    await FetchDataAsync();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            try
            {
                var transactionOutput = await LockTokenVaultManager.GetLockTransactionAsync(LockId);
                var tokenInfo = await TokenManager.GetTokenInfoAsync(transactionOutput.Lock.TokenSymbol);

                Model = new LockTransactionModel(transactionOutput.Lock, transactionOutput.Receivers.ToList(), tokenInfo);
                ShareLink = $"{NavigationManager.BaseUri}view/locks/{LockId}";
                IsLoaded = true;
                MudDialog.SetTitle("");
                StateHasChanged();
            }
            catch
            {
                AppDialogService.ShowError("Lock not found.");
                MudDialog.Cancel();
            }
        }

        private async Task OnCopyShareLinkAsync()
        {
            await ClipboardService.WriteTextAsync(ShareLink);
            AppDialogService.ShowSuccess("Lock Link copied to clipboard.");
        }
    }
}