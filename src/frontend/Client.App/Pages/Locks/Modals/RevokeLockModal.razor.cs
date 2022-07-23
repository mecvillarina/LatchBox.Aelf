using Blazored.FluentValidation;
using Client.App.Parameters;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Locks.Modals
{
    public partial class RevokeLockModal
    {
        [Parameter] public LockOutput Lock { get; set; }
        [Parameter] public RevokeLockParameter Model { get; set; } = new();
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
                    var chain = await ChainService.FetchCurrentChainInfoAsync();

                    var payloadContract = new LockRevokeInput(Model);
                    var txResult = await LockTokenVaultService.RevokeLockAsync(payloadContract);

                    if (txResult != null)
                    {
                        if (txResult.ErrorMessage != null)
                            throw new GeneralException(txResult.ErrorMessage.Message);

                        AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Revoke Lock success. Go to My Refunds Section to claim your token refunds");
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