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
        private (WalletInformation, string) _cred;
        public LockTransactionModel Model { get; set; }
        public string ShareLink { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    await FetchWalletAsync(async (cred) =>
                    {
                        _cred = cred;
                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchWalletAsync(Action<(WalletInformation, string)> action)
        {
            try
            {
                var cred = await WalletManager.GetWalletCredentialsAsync();
                action?.Invoke(cred);
            }
            catch (Exception ex)
            {
                AppDialogService.ShowError(ex.Message);
                MudDialog.Cancel();
            }
        }

        private async Task FetchDataAsync()
        {
            try
            {
                var transactionOutput = await LockTokenVaultManager.GetLockTransactionAsync(_cred.Item1, _cred.Item2, LockId);
                var tokenInfo = await TokenManager.GetTokenInfoAsync(_cred.Item1, _cred.Item2, transactionOutput.Lock.TokenSymbol);

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