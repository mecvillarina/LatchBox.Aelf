using Blazored.FluentValidation;
using Client.Infrastructure.Models;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Shared.Dialogs
{
    public partial class ConfirmWalletTransactionDialog
    {
        public ConfirmWalletTransactionParameter Model { get; set; } = new();
        [Parameter] public string GasDetails { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        
        public WalletInformation Wallet { get; set; }
        public bool PasswordVisibility { get; set; }
        public InputType PasswordInput { get; set; } = InputType.Password;
        public string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    Wallet = await WalletManager.GetWalletInformationAsync();
                    if (Wallet == null)
                    {
                        MudDialog.Cancel();
                        return;
                    }

                    Model.Address = Wallet.Address;

                    StateHasChanged();
                });
            }
        }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                IsProcessing = true;

                try
                {
                    await WalletManager.AuthenticateAsync(Model.Password);
                    MudDialog.Close(DialogResult.Ok((Wallet, Model.Password)));
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                }

                IsProcessing = false;
            }
        }

        private void TogglePasswordVisibility()
        {
            if (PasswordVisibility)
            {
                PasswordVisibility = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                PasswordVisibility = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}