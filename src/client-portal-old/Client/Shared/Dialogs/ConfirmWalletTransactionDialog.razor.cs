using Blazored.FluentValidation;
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

        public string WalletAddress { get; set; }
        public bool PasswordVisibility { get; set; }
        public InputType PasswordInput { get; set; } = InputType.Password;
        public string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    WalletAddress = await WalletManager.GetWalletAddressAsync();
                    if (WalletAddress == null)
                    {
                        MudDialog.Cancel();
                        return;
                    }

                    Model.Address = WalletAddress;

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
                    bool isAuthenticated = await WalletManager.AuthenticateAsync(Model.Password);

                    if (isAuthenticated)
                    {
                        MudDialog.Close(true);
                        return;
                    }

                    AppDialogService.ShowError("Invalid password.");

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