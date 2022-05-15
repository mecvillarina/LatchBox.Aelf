﻿using Client.Models;
using Client.Pages.Vestings.Modals;
using MudBlazor;

namespace Client.Pages.Vestings
{
    public partial class MyVestingRefundsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }

        public List<VestingAssetRefundModel> Refunds { get; set; } = new();

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

            var refundsOutput = await VestingTokenVaultManager.GetRefundsAsync();

            foreach (var refund in refundsOutput.Refunds)
            {
                Refunds.Add(new VestingAssetRefundModel(refund));
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


        private async Task InvokeClaimRefundModalAsync(VestingAssetRefundModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(ClaimVestingRefundModal.Model), model);

            var dialog = DialogService.Show<ClaimVestingRefundModal>($"Claim Refund for {model.TokenInfo.TokenName} ({model.TokenInfo.Symbol})", parameters);
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