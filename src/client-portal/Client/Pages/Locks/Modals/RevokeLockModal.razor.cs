using AElf.Client.LatchBox.LockTokenVault;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks.Modals
{
    public partial class RevokeLockModal
    {
        [Parameter] public Lock Lock { get; set; }
        [Parameter] public RevokeLockParameter Model { get; set; } = new();
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
                        var revokeLockResult = await LockTokenVaultManager.RevokeLockAsync(Model.LockId);

                        if (!string.IsNullOrEmpty(revokeLockResult.Error))
                            throw new GeneralException(revokeLockResult.Error);

                        AppDialogService.ShowSuccess("Revoke Lock success. Go to My Refunds Page to claim your token refunds.");
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