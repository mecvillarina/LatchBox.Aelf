using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks.Modals
{
    public partial class ClaimLockRefundModal
    {
        [Parameter] public LockAssetRefundModel Model { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                IsProcessing = true;

                try
                {
                    var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (authenticated)
                    {
                        var claimRefundResult = await LockTokenVaultManager.ClaimRefundAsync(Model.Refund.TokenSymbol);

                        if (!string.IsNullOrEmpty(claimRefundResult.Error))
                        {
                            throw new GeneralException(claimRefundResult.Error);
                        }
                        else
                        {
                            AppDialogService.ShowSuccess("Claim Refund success.");
                            MudDialog.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                    MudDialog.Cancel();
                }

                IsProcessing = false;
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}