using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Pages.Launchpads.Modals;
using Client.Services;
using MudBlazor;

namespace Client.Pages.Launchpads
{
    public partial class LaunchpadsPage
    {
        public bool IsLoaded { get; set; }

        public TokenInfo NativeTokenInfo { get; set; }
        public List<LaunchpadModel> LaunchpadList { get; set; } = new();
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
                case 1: isUpcoming = true; break;
                case 2: isOngoing = true; break;
                default: isUpcoming = isOngoing = false; break;
            }

            NativeTokenInfo = await TokenManager.GetNativeTokenInfoOnSideChainAsync();
            var output = await MultiCrowdSaleManager.GetCrowdSalesAsync(isUpcoming, isOngoing);
            LaunchpadList = output.List.Select(x => new LaunchpadModel(x)).ToList();
            IsLoaded = true;
            StateHasChanged();
        }

        public async Task OnStatusChanged(int value)
        {
            LaunchpadStatus = value;
            await FetchDataAsync();
        }

        private void OnViewLaunchpad(LaunchpadModel model)
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