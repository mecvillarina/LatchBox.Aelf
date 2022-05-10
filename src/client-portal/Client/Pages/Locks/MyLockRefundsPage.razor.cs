using Client.Models;
using Client.Pages.Locks.Modals;
using MudBlazor;

namespace Client.Pages.Locks
{
    public partial class MyLockRefundsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }

        public List<LockAssetRefundModel> Refunds { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
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

            var refundsOutput = await LockTokenVaultManager.GetRefundsAsync();

            foreach (var refund in refundsOutput.Refunds)
            {
                Refunds.Add(new LockAssetRefundModel(refund));
            }

            IsLoaded = true;
            StateHasChanged();

            foreach (var refund in Refunds)
            {
                var tokenInfo = await TokenManager.GetTokenInfoAsync(refund.Refund.TokenSymbol);
                refund.SetTokenInfo(tokenInfo);
            }

            IsCompletelyLoaded = true;
            StateHasChanged();
        }


        private async Task InvokeClaimRefundModalAsync(LockAssetRefundModel model)
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