using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Pages.CrowdFundings.Modals;
using Client.Pages.Modals;
using Client.Parameters;
using Client.Services;
using MudBlazor;

namespace Client.Pages.CrowdFundings
{
    public partial class ActiveCrowdFundingsPage
    {
        public bool IsLoaded { get; set; }

        private (WalletInformation, string) _creds;
        public TokenInfo NativeTokenInfo { get; set; }
        public List<ActiveCrowdSaleModel> CrowdSaleList { get; set; } = new();

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

            NativeTokenInfo = await TokenManager.GetNativeTokenInfoAsync(_creds.Item1, _creds.Item2);
            await MultiCrowdSaleManager.InitializeAsync(_creds.Item1, _creds.Item2);
            var output = await MultiCrowdSaleManager.GetActiveCrowdSalesAsync(_creds.Item1, _creds.Item2);
            CrowdSaleList = output.CrowdSales.Select(x => new ActiveCrowdSaleModel(x, _creds.Item1)).ToList();
            IsLoaded = true;
            StateHasChanged();
        }

    }
}