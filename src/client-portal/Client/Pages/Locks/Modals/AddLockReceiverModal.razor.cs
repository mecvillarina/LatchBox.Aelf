using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks.Modals
{
    public partial class AddLockReceiverModal
    {
        [Parameter] public TokenInfo TokenInfo { get; set; } = new();
        [Parameter] public AddLockReceiverParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public List<string> WalletAddresses { get; set; } = new();

        private void SubmitAsync()
        {
            if (Validated)
            {
                if (Model.Amount.CountDecimalPlaces() > TokenInfo.Decimals)
                {
                    if (TokenInfo.Decimals > 0)
                    {
                        AppDialogService.ShowError($"Allowable decimal places for 'Amount' less than or equal to {TokenInfo.Decimals}");
                    }
                    else
                    {
                        AppDialogService.ShowError($"{TokenInfo.TokenName} doesn't allow 'Amount' to have decimal places.");
                    }
                }
                else
                {
                    MudDialog.Close(DialogResult.Ok(Model));
                }
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}