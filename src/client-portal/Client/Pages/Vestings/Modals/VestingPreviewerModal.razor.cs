using AElf.Client.LatchBox.VestingTokenVault;
using Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Vestings.Modals
{
    public partial class VestingPreviewerModal
    {
        [Parameter] public long VestingId { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsLoaded { get; set; }
        public VestingModel Model { get; set; }
        public string ShareLink { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    await FetchDataAsync();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            try
            {
                var output = await VestingTokenVaultManager.GetVestingTransactionAsync(VestingId);
                var tokenInfo = await TokenManager.GetTokenInfoAsync(output.Vesting.TokenSymbol);

                Model = new VestingModel(output, tokenInfo);
                ShareLink = $"{NavigationManager.BaseUri}view/vestings/{VestingId}";
                IsLoaded = true;
                MudDialog.SetTitle("");
                StateHasChanged();
            }
            catch
            {
                AppDialogService.ShowError("Vesting not found.");
                MudDialog.Cancel();
            }
        }

        private void InvokeVestingPeriodPreviewerModal(VestingTransactionPeriodOutput periodOutput)
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(VestingPeriodPreviewerModal.TokenInfo), Model.TokenInfo);
            parameters.Add(nameof(VestingPeriodPreviewerModal.Period), periodOutput.Period);
            parameters.Add(nameof(VestingPeriodPreviewerModal.Receivers), periodOutput.Receivers.ToList());
            parameters.Add(nameof(VestingPeriodPreviewerModal.Vesting), Model.Vesting);

            var options = new DialogOptions() { CloseButton = true };
            DialogService.Show<VestingPeriodPreviewerModal>($"Vesting Period", parameters, options);
        }

        private async Task OnCopyShareLinkAsync()
        {
            await ClipboardService.WriteTextAsync(ShareLink);
            AppDialogService.ShowSuccess("Vesting Link copied to clipboard.");
        }
    }
}