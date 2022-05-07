using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Pages.CrowdFundings.Modals;
using Client.Pages.Modals;
using Client.Parameters;
using Client.Services;
using MudBlazor;

namespace Client.Pages.CrowdFundings
{
    public partial class MyCrowdFundingsPage
    {
        public bool IsLoaded { get; set; }

        private (WalletInformation, string) _creds;
        public TokenInfo NativeTokenInfo { get; set; }
        public List<MyCrowdSaleModel> CrowdSaleList { get; set; } = new();

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
            var output = await MultiCrowdSaleManager.GetCrowdSalesByInitiatorAsync(_creds.Item1, _creds.Item2, _creds.Item1.Address);
            CrowdSaleList = output.CrowdSales.Select(x => new MyCrowdSaleModel(x)).ToList();

            IsLoaded = true;
            StateHasChanged();
        }

        private async Task InvokeAddTokenModalAsync()
        {
            var searchTokenDialog = DialogService.Show<SearchTokenModal>($"Search Token");
            var searchTokenDialogResult = await searchTokenDialog.Result;

            if (!searchTokenDialogResult.Cancelled)
            {
                var data = (TokenInfo)searchTokenDialogResult.Data;

                var createCrowdSaleParameters = new DialogParameters()
                {
                    { nameof(CreateCrowdSaleModal.Model), new CreateCrowdSaleParameter()
                        {
                           NativeTokenName = NativeTokenInfo.TokenName,
                           NativeTokenSymbol = NativeTokenInfo.Symbol,
                           NativeTokenDecimals = NativeTokenInfo.Decimals,
                           TokenName = data.TokenName,
                           TokenSymbol = data.Symbol,
                           TokenDecimals = data.Decimals
                        }
                    }
                };

                var createCrowdSaleDialog = DialogService.Show<CreateCrowdSaleModal>("Create Crowd Funding", createCrowdSaleParameters);
                var createCrowdSaleDialogResult = await createCrowdSaleDialog.Result;

                if (!createCrowdSaleDialogResult.Cancelled)
                {
                    await FetchDataAsync();
                }
            }
        }

        private async Task InvokeCancelCrowdSaleAsync(MyCrowdSaleModel output)
        {
            var parameters = new DialogParameters()
            {
                { nameof(CancelCrowdSaleConfirmationModal.Model), output},
                { nameof(CancelCrowdSaleConfirmationModal.NativeTokenInfo), NativeTokenInfo}
            };

            var dialog = DialogService.Show<CancelCrowdSaleConfirmationModal>("Cancel Crowd Funding Confirmation", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

    }
}