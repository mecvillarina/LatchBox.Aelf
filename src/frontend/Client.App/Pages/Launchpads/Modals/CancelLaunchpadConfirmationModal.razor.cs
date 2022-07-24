using Client.App.Models;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.Launchpad;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads.Modals
{
    public partial class CancelLaunchpadConfirmationModal
    {
        [Parameter] public MyLaunchpadModel Model { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            try
            {
                IsProcessing = true;

                var chain = await ChainService.FetchCurrentChainInfoAsync();

                var payloadContract = new CrowdSaleCancelInput() { CrowdSaleId = Model.Launchpad.Id };
                var txResult = await LaunchpadService.CancelAsync(payloadContract);

                if (txResult != null)
                {
                    if (txResult.ErrorMessage != null)
                        throw new GeneralException(txResult.ErrorMessage.Message);

                    AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Launchpad cancellation success");
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

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}