using Blazored.FluentValidation;
using Client.App.Models;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Locks.Modals
{
    public partial class ClaimLockRefundModal
    {
        [Parameter] public LockRefundModel Model { get; set; }
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
                    var chain = await ChainService.FetchCurrentChainInfoAsync();

                    var payloadContract = new LockClaimRefundInput(Model);
                    var txResult = await LockTokenVaultService.ClaimRefundAsync(payloadContract);

                    if (txResult != null)
                    {
                        if (txResult.ErrorMessage != null)
                            throw new GeneralException(txResult.ErrorMessage.Message);

                        AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Claim refund success");
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