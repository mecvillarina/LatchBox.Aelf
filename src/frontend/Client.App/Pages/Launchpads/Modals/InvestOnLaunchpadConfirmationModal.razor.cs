using Application.Common.Extensions;
using Blazored.FluentValidation;
using Client.App.Models;
using Client.App.Parameters;
using Client.App.Services;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.Launchpad;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads.Modals
{
    public partial class InvestOnLaunchpadConfirmationModal
    {
        [Parameter] public InvestOnLaunchpadParameter Model { get; set; }
        [Parameter] public LaunchpadModel LaunchpadModel { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                if (Model.Amount.CountDecimalPlaces() > NativeTokenInfo.Decimals)
                {
                    if (NativeTokenInfo.Decimals > 0)
                    {
                        AppDialogService.ShowError($"Allowable decimal places for 'Amount' less than or equal to {NativeTokenInfo.Decimals}");
                    }
                    else
                    {
                        AppDialogService.ShowError($"{NativeTokenInfo.TokenName} doesn't allow 'Amount' to have decimal places.");
                    }

                    return;
                }

                try
                {
                    IsProcessing = true;

                    var chain = await ChainService.FetchCurrentChainInfoAsync();
                    var walletAddress = await NightElfService.GetAddressAsync();
                    var amount = Model.Amount.ToChainAmount(NativeTokenInfo.Decimals);

                    var getAllowanceResult = await TokenService.GetAllowanceAsync(new SmartContractDto.TokenGetAllowanceInput()
                    {
                        Symbol = NativeTokenInfo.Symbol,
                        Owner = walletAddress,
                        Spender = chain.LaunchpadContractAddress
                    });

                    bool isApprove = true;

                    if (getAllowanceResult.Allowance < amount)
                    {
                        isApprove = false;
                        var approveTxResult = await TokenService.ApproveAsync(new TokenApproveInput()
                        {
                            Amount = amount,
                            Symbol = NativeTokenInfo.Symbol,
                            Spender = chain.LaunchpadContractAddress
                        });

                        if (approveTxResult != null)
                        {
                            if (approveTxResult.ErrorMessage != null)
                                throw new GeneralException(approveTxResult.ErrorMessage.Message);

                            isApprove = true;
                        }
                    }

                    if (isApprove)
                    {
                        var txResult = await LaunchpadService.InvestAsync(new CrowdSaleInvestInput()
                        {
                            CrowdSaleId = LaunchpadModel.Launchpad.Id,
                            TotalAmount = amount
                        });

                        if (txResult != null)
                        {
                            if (txResult.ErrorMessage != null)
                                throw new GeneralException(txResult.ErrorMessage.Message);

                            AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Launchpad invest success");
                            MudDialog.Close();
                        }
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