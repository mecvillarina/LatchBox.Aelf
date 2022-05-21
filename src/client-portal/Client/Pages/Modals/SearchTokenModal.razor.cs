using Blazored.FluentValidation;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Modals
{
    public partial class SearchTokenModal
    {
        [Parameter] public SearchTokenParameter Model { get; set; } = new();
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
                    var tokenInfo = await TokenManager.GetTokenInfoAsync(Model.Symbol.ToUpper());

                    if (!string.IsNullOrEmpty(tokenInfo.Symbol))
                    {
                        MudDialog.Close(DialogResult.Ok(tokenInfo));
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

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}