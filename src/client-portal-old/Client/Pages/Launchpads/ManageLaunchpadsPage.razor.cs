using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
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

        public TokenInfo NativeTokenInfo { get; set; }
        public string WalletAddress { get; set; }
        public List<MyLaunchpadModel> LaunchpadList { get; set; } = new();
        public string SideChain { get; set; }
        public string ContractLink => $"{BlockchainManager.SideChainExplorer}/address/{MultiCrowdSaleManager.ContactAddress}";

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        WalletAddress = await WalletManager.GetWalletAddressAsync();
                        SideChain = $"Side {BlockchainManager.GetSideChainId().ToChainName()}";

                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            NativeTokenInfo = await TokenManager.GetNativeTokenInfoOnSideChainAsync();
            var output = await MultiCrowdSaleManager.GetCrowdSalesByInitiatorAsync(WalletAddress);
            LaunchpadList = output.List.Select(x => new MyLaunchpadModel(x)).ToList();

            IsLoaded = true;
            StateHasChanged();
        }

        private async Task InvokeAddLaunchpadModalAsync()
        {
            var searchTokenDialog = DialogService.Show<SearchTokenModal>($"Search Token ({SideChain} Chain)");
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

        private async Task InvokeCompleteLaunchpadConfirmationAsync(MyLaunchpadModel model)
        {
            var parameters = new DialogParameters()
            {
                { nameof(CompleteLaunchpadConfirmationModal.Model), model},
                { nameof(CompleteLaunchpadConfirmationModal.NativeTokenInfo), NativeTokenInfo}
            };

            var dialog = DialogService.Show<CompleteLaunchpadConfirmationModal>("Complete Launchpad Confirmation", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private void OnViewLaunchpad(MyLaunchpadModel model)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.CrowdSaleId), model.Launchpad.Id},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"{model.Launchpad.Name}", parameters, options);
        }
    }
}