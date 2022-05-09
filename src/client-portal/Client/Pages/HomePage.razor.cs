using Client.Infrastructure.Managers.Interfaces;
using Client.Pages.Locks.Modals;
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

                    if (LockId.HasValue && LockId.Value >= 0)
                    {
                        InvokeLockPreviewerModal(LockId.Value);
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

        //private void InvokeVestingPreviewerModal(BigInteger vestingIndex)
        //{
        //    var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
        //    var parameters = new DialogParameters()
        //    {
        //         { nameof(VestingPreviewerModal.VestingIndex), vestingIndex},
        //    };

        //    DialogService.Show<VestingPreviewerModal>($"Vesting #{vestingIndex}", parameters, options);
        //}
    }
}