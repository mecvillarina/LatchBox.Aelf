using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Numerics;

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
            try
            {
                var crowdSale = await MultiCrowdSaleManager.GetCrowdSaleAsync(_creds.Item1, _creds.Item2, Model.CrowdSale.Id);
                Model = new CrowdSaleModel(crowdSale);
                //ShareLink = $"{NavigationManager.BaseUri}view/locks/{LockIndex}";
                IsLoaded = true;
                StateHasChanged();
            }
            catch
            {
                //AppDialogService.ShowError("LatchBox Lock not found.");
                MudDialog.Cancel();
            }
        }

        private async Task OnCopyShareLinkAsync()
        {
            await ClipboardService.WriteTextAsync(ShareLink);
            AppDialogService.ShowSuccess("Link copied to clipboard.");
        }
    }
}