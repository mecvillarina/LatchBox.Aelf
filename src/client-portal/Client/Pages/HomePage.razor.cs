using Client.Infrastructure.Extensions;
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

        public string MainChain { get; set; }
        public string SideChain { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsDataLoaded { get; set; }

        //Node
        public string MainChainNode => BlockchainManager.MainChainNode;
        public string SideChainNode => BlockchainManager.SideChainNode;

        //Lock
        public string LockContractExplorerLink => $"{BlockchainManager.SideChainExplorer}/address/{LockTokenVaultManager.ContactAddress}";
        public long LockTotalCount { get; set; }
        public long LockedTokenCount { get; set; }

        //Vesting
        public string VestingContractExplorerLink => $"{BlockchainManager.SideChainExplorer}/address/{VestingTokenVaultManager.ContactAddress}";
        public long VestingTotalCount { get; set; }

        //Launchpad
        public string LaunchpadContractExplorerLink => $"{BlockchainManager.SideChainExplorer}/address/{MultiCrowdSaleManager.ContactAddress}";
        public long LaunchpadUpcomingCount { get; set; }
        public long LaunchpadOngoingCount { get; set; }
        public long LaunchpadTotalCount { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    if (LockId.HasValue && LockId.Value > 0)
                    {
                        InvokeLockPreviewerModal(LockId.Value);
                    }

                    if (VestingId.HasValue && VestingId.Value > 0)
                    {
                        InvokeVestingPreviewerModal(LockId.Value);
                    }

                    await InvokeAsync(async () =>
                    {
                        await InitAsync();
                        await FetchDataAsync();
                    });
                });

            }
        }

        private async Task InitAsync()
        {
            IsLoaded = false;
            StateHasChanged();
            MainChain = (await BlockchainManager.GetMainChainIdAsync()).ToStringChainId();
            SideChain = (await BlockchainManager.GetSideChainIdAsync()).ToStringChainId();

            IsLoaded = true;
            StateHasChanged();
        }

        private async Task FetchDataAsync()
        {
            IsDataLoaded = false;

            try
            {
                LockTotalCount = (await LockTokenVaultManager.GetLocksCountAsync()).Value;
                LockedTokenCount = (await LockTokenVaultManager.GetAssetsCounterAsync()).Assets.Count;
                VestingTotalCount = (await VestingTokenVaultManager.GetVestingsCountAsync()).Value;

                //await MultiCrowdSaleManager.InitializeAsync();
                //var s = await LockTokenVaultManager.InitializeAsync();
                //var s = await VestingTokenVaultManager.InitializeAsync();
            }
            catch
            {

            }

            AppDialogService.ShowSuccess("Success");
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
    }
}