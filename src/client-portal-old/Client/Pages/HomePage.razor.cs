using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Pages.Launchpads.Modals;
using Client.Pages.Locks.Modals;
using Client.Pages.Vestings.Modals;
using Client.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages
{
    public partial class HomePage
    {
        [Parameter]
        public long? LockId { get; set; }

        [Parameter]
        public long? VestingId { get; set; }

        [Parameter]
        public long? CrowdSaleId { get; set; }

        public string MainChain { get; set; }
        public string MainChainBestHeight { get; set; }
        public string SideChain { get; set; }
        public string SideChainBestHeight { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsDataLoaded { get; set; }

        public TokenInfo NativeTokenInfo { get; set; }

        //Node
        public string MainChainNode => BlockchainManager.MainChainNode;
        public string SideChainNode => BlockchainManager.SideChainNode;

        //Lock
        public string LockContractExplorerLink => $"{BlockchainManager.SideChainExplorer}/address/{LockTokenVaultManager.ContactAddress}";
        public long? LockTotalCount { get; set; }
        public long? LockedTokenCount { get; set; }

        //Vesting
        public string VestingContractExplorerLink => $"{BlockchainManager.SideChainExplorer}/address/{VestingTokenVaultManager.ContactAddress}";
        public long? VestingTotalCount { get; set; }

        //Launchpad
        public string LaunchpadContractExplorerLink => $"{BlockchainManager.SideChainExplorer}/address/{MultiCrowdSaleManager.ContactAddress}";
        public long? LaunchpadUpcomingCount { get; set; }
        public long? LaunchpadOngoingCount { get; set; }
        public long? LaunchpadTotalCount { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InitAsync();

                    if (LockId.HasValue && LockId.Value > 0)
                    {
                        InvokeLockPreviewerModal(LockId.Value);
                    }

                    if (VestingId.HasValue && VestingId.Value > 0)
                    {
                        InvokeVestingPreviewerModal(LockId.Value);
                    }

                    if (CrowdSaleId.HasValue && CrowdSaleId.Value > 0)
                    {
                        InvokeLaunchpadPreviewerModal(CrowdSaleId.Value);
                    }

                    await FetchDataAsync();
                });

            }
        }

        private async Task InitAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            MainChain = BlockchainManager.GetMainChainId().ToChainName();
            MainChainBestHeight = BlockchainManager.FetchMainChainStatus().BestChainHeight.ToString();
            SideChain = BlockchainManager.GetSideChainId().ToChainName();
            SideChainBestHeight = BlockchainManager.FetchSideChainStatus().BestChainHeight.ToString();

            if (CrowdSaleId.HasValue && NativeTokenInfo == null)
            {
                NativeTokenInfo = await TokenManager.GetNativeTokenInfoOnMainChainAsync();
            }

            IsLoaded = true;
            StateHasChanged();
        }

        private async Task FetchDataAsync()
        {
            IsDataLoaded = false;

            try
            {
                await InitAsync();

                LockTotalCount = null;
                LockedTokenCount = null;
                VestingTotalCount = null;
                LaunchpadTotalCount = null;
                LaunchpadUpcomingCount = null;
                LaunchpadOngoingCount = null;

                StateHasChanged();

                var tasks = new List<Task>();

                tasks.Add(InvokeAsync(async () =>
                {
                    var output = await LockTokenVaultManager.GetLocksCountAsync();
                    LockTotalCount = output.Value;
                    StateHasChanged();
                }));

                tasks.Add(InvokeAsync(async () =>
                {
                    var output = await LockTokenVaultManager.GetAssetsCounterAsync();
                    LockedTokenCount = output.Assets.Count;
                    StateHasChanged();
                }));

                tasks.Add(InvokeAsync(async () =>
                {
                    var output = await VestingTokenVaultManager.GetVestingsCountAsync();
                    VestingTotalCount = output.Value;
                    StateHasChanged();
                }));

                tasks.Add(InvokeAsync(async () =>
                {
                    var output = await MultiCrowdSaleManager.GetCrowdSaleCountAsync();
                    LaunchpadTotalCount = output.Value;
                    StateHasChanged();
                }));

                tasks.Add(InvokeAsync(async () =>
                {
                    var output = await MultiCrowdSaleManager.GetUpcomingCrowdSaleCountAsync();
                    LaunchpadUpcomingCount = output.Value;
                    StateHasChanged();
                }));

                tasks.Add(InvokeAsync(async () =>
                {
                    var output = await MultiCrowdSaleManager.GetOngoingCrowdSaleCountAsync();
                    LaunchpadOngoingCount = output.Value;
                    StateHasChanged();
                }));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {

            }

            IsDataLoaded = true;
            StateHasChanged();
        }

        private void InvokeLockPreviewerModal(long lockId)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LockPreviewerModal.LockId), lockId},
            };

            DialogService.Show<LockPreviewerModal>($"Lock #{lockId}", parameters, options);
        }

        private void InvokeVestingPreviewerModal(long vestingId)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(VestingPreviewerModal.VestingId), vestingId},
            };

            DialogService.Show<VestingPreviewerModal>($"Vesting #{vestingId}", parameters, options);
        }


        private void InvokeLaunchpadPreviewerModal(long crowdSaleId)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.CrowdSaleId), crowdSaleId},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"Launchpad #{crowdSaleId}", parameters, options);
        }
    }
}