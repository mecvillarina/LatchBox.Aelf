using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Pages.Launchpads.Modals;
using Client.Pages.Locks.Modals;
using Client.Pages.Modals;
using Client.Parameters;
using Client.Services;
using MudBlazor;

namespace Client.Pages.Launchpads
{
    public partial class InvestedLaunchpadsPage
    {
        public bool IsLoaded { get; set; }

        public TokenInfo NativeTokenInfo { get; set; }
        public WalletInformation Wallet { get; set; }
        public List<InvestedLaunchpadModel> LaunchpadList { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        Wallet = await WalletManager.GetWalletInformationAsync();
                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            NativeTokenInfo = await TokenManager.GetNativeTokenInfoAsync();
            var output = await MultiCrowdSaleManager.GetCrowdSalesByInvestorAsync(Wallet.Address);
            LaunchpadList = output.List.Select(x => new InvestedLaunchpadModel(x)).ToList();

            IsLoaded = true;
            StateHasChanged();
        }

        private void OnViewLaunchpad(InvestedLaunchpadModel model)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.CrowdSaleId), model.Launchpad.Id},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"{model.Launchpad.Name}", parameters, options);
        }

        private void InvokeLockPreviewerModal(long lockId)
        {
            NavigationManager.NavigateTo($"/locks/claims/{lockId}");
        }

    }
}