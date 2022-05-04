using Client.Infrastructure.Managers.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    public partial class HomePage
    {
        [Inject] public ITokenManager TokenManager { get; set; }

        [Parameter]
        public long? LockIndex { get; set; }

        [Parameter]
        public long? VestingIndex { get; set; }

        public bool IsLoaded { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //if (LockIndex.HasValue && LockIndex.Value >= 0)
                //{
                //    InvokeLockPreviewerModal(LockIndex.Value);
                //}
                //else if (VestingIndex.HasValue && VestingIndex.Value >= 0)
                //{
                //    InvokeVestingPreviewerModal(VestingIndex.Value);
                //}

                await InvokeAsync(async () =>
                {
                    await FetchDataAsync();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;

            StateHasChanged();

            var authenticated = await AuthManager.IsAuthenticated();

            if (authenticated)
            {
                var cred = await WalletManager.GetWalletCrdentialsAsync();
                //await TokenManager.GetBalanceAsync(cred.Item1, cred.Item2, "LATCH");
                //await TokenManager.GetTokenInfoAsync(cred.Item1, cred.Item2, "ELF");
                await TokenManager.CreateTokenAsync(cred.Item1, cred.Item2);
            }

            IsLoaded = true;
            StateHasChanged();
        }

        //private void InvokeLockPreviewerModal(BigInteger lockIndex)
        //{
        //    var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
        //    var parameters = new DialogParameters()
        //    {
        //         { nameof(LockPreviewerModal.LockIndex), lockIndex},
        //    };

        //    DialogService.Show<LockPreviewerModal>($"Lock #{lockIndex}", parameters, options);
        //}

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