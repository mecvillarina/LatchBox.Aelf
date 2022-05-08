using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.CrowdFundings.Modals
{
    public partial class CancelCrowdSaleConfirmationModal
    {
        [Parameter] public MyCrowdSaleModel Model { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            try
            {
                IsProcessing = true;

                var cred = await AppDialogService.ShowConfirmWalletTransactionAsync();

                if (cred.Item1 != null)
                {
                    var createCrowdSaleResult = await MultiCrowdSaleManager.CancelAsync(cred.Item1, cred.Item2, Model.CrowdSale.Id);

                    if (!string.IsNullOrEmpty(createCrowdSaleResult.Error))
                    {
                        throw new GeneralException(createCrowdSaleResult.Error);
                    }
                    else
                    {
                        AppDialogService.ShowSuccess("Launchpad cancellation success.");
                        MudDialog.Close();
                    }
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