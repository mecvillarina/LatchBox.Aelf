using Application.Common.Extensions;
using Blazored.FluentValidation;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Assets.Modals
{
    public partial class IssueTokenModal
    {
        [Parameter] public IssueTokenParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                try
                {
                    if (Model.Amount.CountDecimalPlaces() > Model.Decimals)
                    {
                        if (Model.Decimals > 0)
                        {
                            AppDialogService.ShowError($"Allowable decimal places for 'Amount' less than or equal to {Model.Decimals}");
                        }
                        else
                        {
                            AppDialogService.ShowError($"{Model.TokenName} doesn't allow 'Amount' to have decimal places.");
                        }

                        return;
                    }

                    IsProcessing = true;

                    var chain = await ChainService.FetchCurrentChainInfoAsync();

                    var amount = Model.Amount.ToChainAmount(Model.Decimals);
                    var payloadContract = new IssueTokenInput(Model) { Amount = amount };
                    var txResult = await NightElfService.SendTxAsync(chain.TokenContractAddress, "Issue", payloadContract);

                    if (txResult != null)
                    {
                        if (txResult.ErrorMessage != null)
                            throw new GeneralException(txResult.ErrorMessage.Message);

                        AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Issue token success");
                        MudDialog.Close();
                    }
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                }

                IsProcessing = false;
            }
        }

        private async Task SetToValueAsync()
        {
            var isConnected = await NightElfService.IsConnectedAsync();
            if (isConnected)
            {
                var walletAddress = await NightElfService.GetAddressAsync();
                Model.To = walletAddress;
                StateHasChanged();
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}