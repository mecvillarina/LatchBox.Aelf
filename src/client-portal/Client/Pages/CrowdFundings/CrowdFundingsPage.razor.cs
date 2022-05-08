using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Pages.CrowdFundings.Modals;
using Client.Services;
using MudBlazor;

namespace Client.Pages.CrowdFundings
{
    public partial class CrowdFundingsPage
    {
        public bool IsLoaded { get; set; }

        private (WalletInformation, string) _creds;
        public TokenInfo NativeTokenInfo { get; set; }
        public List<CrowdSaleModel> CrowdSaleList { get; set; } = new();
        public int LaunchpadStatus { get; set; }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        _creds = await WalletManager.GetWalletCredentialsAsync();
                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            bool isUpcoming = false;
            bool isOngoing = false;

            switch (LaunchpadStatus)
            {
                case 1: isUpcoming = true;break;
                case 2: isOngoing = true; break;
                default: isUpcoming = isOngoing = false; break;
            }

            NativeTokenInfo = await TokenManager.GetNativeTokenInfoAsync(_creds.Item1, _creds.Item2);
            await MultiCrowdSaleManager.InitializeAsync(_creds.Item1, _creds.Item2);
            var output = await MultiCrowdSaleManager.GetCrowdSalesAsync(_creds.Item1, _creds.Item2, isUpcoming, isOngoing);
            CrowdSaleList = output.CrowdSales.Select(x => new CrowdSaleModel(x)).ToList();
            IsLoaded = true;
            StateHasChanged();
        }

        public async Task OnStatusChanged(int value)
        {
            LaunchpadStatus = value;
            await FetchDataAsync();
        }

        private void OnViewLaunchpad(CrowdSaleModel model)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.Model), model},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"{model.CrowdSale.Name}", parameters, options);
        }
    }
}