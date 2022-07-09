using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Launchpads.Modals
{
    public partial class CancelLaunchpadConfirmationModal
    {
        [Parameter] public MyLaunchpadModel Model { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            try
            {
                IsProcessing = true;

                var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                if (authenticated)
                {
                    var cancelResult = await MultiCrowdSaleManager.CancelAsync(Model.Launchpad.Id);

                    if (!string.IsNullOrEmpty(cancelResult.Error))
                        throw new GeneralException(cancelResult.Error);

                    AppDialogService.ShowSuccess("Launchpad cancellation success.");
                    MudDialog.Close();
                }
            }
            catch (Exception ex)
            {
                AppDialogService.ShowError(ex.Message);
            }

            IsProcessing = false;
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}