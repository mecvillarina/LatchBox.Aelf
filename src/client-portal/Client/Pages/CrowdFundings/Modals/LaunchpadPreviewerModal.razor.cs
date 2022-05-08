using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.CrowdFundings.Modals
{
    public partial class LaunchpadPreviewerModal
    {
        [Parameter] public CrowdSaleModel Model { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsLoaded { get; set; }
        public string ShareLink { get; set; }
        private (WalletInformation, string) _creds;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    _creds = await WalletManager.GetWalletCredentialsAsync();
                    await FetchDataAsync();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            var crowdSale = await MultiCrowdSaleManager.GetCrowdSaleAsync(_creds.Item1, _creds.Item2, Model.CrowdSale.Id);
            Model = new CrowdSaleModel(crowdSale);
            
            IsLoaded = true;
            StateHasChanged();
        }

        private async Task OnCopyShareLinkAsync()
        {
            await ClipboardService.WriteTextAsync(ShareLink);
            AppDialogService.ShowSuccess("Link copied to clipboard.");
        }

        private async Task InvokeInvestOnCrowdSaleModalAsync()
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
            var parameters = new DialogParameters()
            {
                 { nameof(InvestOnCrowdSaleConfirmationModal.CrowdSaleModel), Model},
                 { nameof(InvestOnCrowdSaleConfirmationModal.NativeTokenInfo), NativeTokenInfo},
                 { nameof(InvestOnCrowdSaleConfirmationModal.Model), new InvestOnCrowdSaleParameter() { LimitAmount = (double)Model.CrowdSale.NativeTokenPurchaseLimitPerBuyerAddress.ToAmount(NativeTokenInfo.Decimals) } },
            };

            var dialog = DialogService.Show<InvestOnCrowdSaleConfirmationModal>($"{Model.CrowdSale.Name} invest confirmation", parameters, options);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }
    }
}