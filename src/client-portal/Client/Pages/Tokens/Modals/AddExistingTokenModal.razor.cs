using Blazored.FluentValidation;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Tokens.Modals
{
    public partial class AddExistingTokenModal
    {
        [Parameter] public AddExistingTokenParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                IsProcessing = true;

                try
                {
                    var cred = await WalletManager.GetWalletCredentialsAsync();
                    var tokenInfo = await TokenManager.GetTokenInfoAsync(cred.Item1, cred.Item2, Model.Symbol.ToUpper());

                    if (!string.IsNullOrEmpty(tokenInfo.Symbol))
                    {
                        await TokenManager.AddTokenSymbolToStorageAsync(Model.Symbol.ToUpper());
                        MudDialog.Close();
                    }
                    else
                    {
                        AppDialogService.ShowError("Token not found.");
                    }
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                }

                IsProcessing = false;
            }
        }

        private Exception GeneralException(string error)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}