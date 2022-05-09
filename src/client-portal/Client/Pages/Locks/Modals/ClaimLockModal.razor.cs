using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks.Modals
{
    public partial class ClaimLockModal
    {
        [Parameter] public ClaimLockParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
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
                        var claimLockResult = await LockTokenVaultManager.ClaimLockAsync(Model.LockId);

                        if (!string.IsNullOrEmpty(claimLockResult.Error))
                        {
                            throw new GeneralException(claimLockResult.Error);
                        }
                        else
                        {
                            AppDialogService.ShowSuccess("Claim Lock success.");
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