using Client.Infrastructure.Managers.Interfaces;
using Client.Pages.Locks.Modals;
using Client.Pages.Vestings.Modals;
using Client.Services;
using Google.Protobuf.WellKnownTypes;
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

        public bool IsLoaded { get; set; }

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
                        await FetchDataAsync();
                    });
                });

            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;

            try
            {
                //await MultiCrowdSaleManager.InitializeAsync();
                //var s = await LockTokenVaultManager.InitializeAsync();
                //var s = await VestingTokenVaultManager.InitializeAsync();
            }
            catch
            {

            }

            AppDialogService.ShowSuccess("Success");
            IsLoaded = true;
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