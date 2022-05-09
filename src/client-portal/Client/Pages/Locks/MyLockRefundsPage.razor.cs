using Client.Infrastructure.Models;
using Client.Models;
using Client.Pages.Locks.Modals;
using MudBlazor;

namespace Client.Pages.Locks
{
    public partial class MyLockRefundsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }

        private (WalletInformation, string) _cred;
        public List<AssetRefundModel> Refunds { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        _cred = await WalletManager.GetWalletCredentialsAsync();
                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            IsCompletelyLoaded = false;
            StateHasChanged();

            Refunds.Clear();

            var refundsOutput = await LockTokenVaultManager.GetRefundsAsync(_cred.Item1, _cred.Item2);

            foreach (var refund in refundsOutput.Refunds)
            {
                Refunds.Add(new AssetRefundModel(refund));
            }

            IsLoaded = true;
            StateHasChanged();

            foreach (var refund in Refunds)
            {
                var tokenInfo = await TokenManager.GetTokenInfoAsync(_cred.Item1, _cred.Item2, refund.Refund.TokenSymbol);
                refund.SetTokenInfo(tokenInfo);
            }

            IsCompletelyLoaded = true;
            StateHasChanged();
        }


        private async Task InvokeClaimRefundModalAsync(AssetRefundModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(ClaimLockRefundModal.Model), model);

            var dialog = DialogService.Show<ClaimLockRefundModal>($"Claim Refund for {model.TokenInfo.TokenName} ({model.TokenInfo.Symbol})", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private async Task OnTextToClipboardAsync(string text)
        {
            await ClipboardService.WriteTextAsync(text);
            AppDialogService.ShowSuccess("Contract Address copied to clipboard.");
        }

    }
}