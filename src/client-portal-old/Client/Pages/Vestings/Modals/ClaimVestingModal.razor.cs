using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Vestings.Modals
{
    public partial class ClaimVestingModal
    {
        [Parameter] public ClaimVestingParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsLoaded = true;
                StateHasChanged();
            }
        }

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
                        var claimResult = await VestingTokenVaultManager.ClaimVestingAsync(Model.VestingId, Model.PeriodId);

                        if (!string.IsNullOrEmpty(claimResult.Error))
                            throw new GeneralException(claimResult.Error);

                        AppDialogService.ShowSuccess("Claim Vesting success.");
                        MudDialog.Close();
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