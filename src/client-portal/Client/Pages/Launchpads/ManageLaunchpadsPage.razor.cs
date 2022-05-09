using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Pages.Launchpads.Modals;
using Client.Pages.Modals;
using Client.Parameters;
using Client.Services;
using MudBlazor;

namespace Client.Pages.Launchpads
{
    public partial class ManageLaunchpadsPage
    {
        public bool IsLoaded { get; set; }

        private (WalletInformation, string) _creds;
        public TokenInfo NativeTokenInfo { get; set; }
        public List<MyLaunchpadModel> LaunchpadList { get; set; } = new();

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
            LaunchpadList = output.CrowdSales.Select(x => new MyLaunchpadModel(x)).ToList();

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

                var createParameters = new DialogParameters()
                {
                    { nameof(CreateLaunchpadModal.Model), new CreateLaunchpadParameter()
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

                var createDialog = DialogService.Show<CreateLaunchpadModal>("Create Launchpad", createParameters);
                var createDialogResult = await createDialog.Result;

                if (!createDialogResult.Cancelled)
                {
                    await FetchDataAsync();
                }
            }
        }

        private async Task InvokeCancelLaunchpadConfirmationAsync(MyLaunchpadModel output)
        {
            var parameters = new DialogParameters()
            {
                { nameof(CancelLaunchpadConfirmationModal.Model), output},
                { nameof(CancelLaunchpadConfirmationModal.NativeTokenInfo), NativeTokenInfo}
            };

            var dialog = DialogService.Show<CancelLaunchpadConfirmationModal>("Cancel Launchpad Confirmation", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

    }
}