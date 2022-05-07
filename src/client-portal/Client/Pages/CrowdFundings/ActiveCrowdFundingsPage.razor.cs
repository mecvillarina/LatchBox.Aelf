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
        public bool IsCompletelyLoaded { get; set; }

        private (WalletInformation, string) _creds;
        private TokenInfo _nativeTokenInfo;
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
            IsCompletelyLoaded = false;
            StateHasChanged();

            _nativeTokenInfo = await TokenManager.GetNativeTokenInfoAsync(_creds.Item1, _creds.Item2);
            //var result = await MultiCrowdSaleManager.CreateAsync(_creds.Item1, _creds.Item2, new CreateCrowdSaleInputModel()
            //{
            //    Name = "PreSale - LATCH",
            //    Pausable = true,
            //    SoftCapNativeTokenAmount = 100000000,
            //    HardCapNativeTokenAmount = 1000000000,
            //    NativeTokenLimitPerSale = 50000000,
            //    SaleEndDate = DateTime.UtcNow.AddDays(1),
            //    UnlockDate = DateTime.UtcNow.AddDays(2),
            //    TokenSymbol = "LATCH",
            //    TokenAmountPerNativeToken = 10000000000
            //});

            IsLoaded = true;
            StateHasChanged();

            IsCompletelyLoaded = true;
            StateHasChanged();
        }

    }
}