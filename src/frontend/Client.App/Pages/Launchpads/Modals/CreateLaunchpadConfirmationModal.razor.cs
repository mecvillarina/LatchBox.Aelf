using Application.Common.Extensions;
using Blazored.FluentValidation;
using Client.App.Parameters;
using Client.App.SmartContractDto.Launchpad;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads.Modals
{
    public partial class CreateLaunchpadConfirmationModal
    {
        [Parameter] public CreateLaunchpadParameter Model { get; set; } = new();
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
                    IsProcessing = true;

                    var chain = await ChainService.FetchCurrentChainInfoAsync();
                    var walletAddress = await NightElfService.GetAddressAsync();
                    var amount = (long)(Model.TokenAmountPerNativeToken.ToChainAmount(Model.TokenDecimals) * Model.HardCapNativeTokenAmount);

                    var getAllowanceResult = await TokenService.GetAllowanceAsync(new SmartContractDto.TokenGetAllowanceInput()
                    {
                        Symbol = Model.TokenSymbol,
                        Owner = walletAddress,
                        Spender = chain.LaunchpadContractAddress
                    });

                    bool isApprove = true;

                    if (getAllowanceResult.Allowance < amount)
                    {
                        isApprove = false;
                        var approveTxResult = await TokenService.ApproveAsync(new SmartContractDto.TokenApproveInput()
                        {
                            Amount = amount,
                            Symbol = Model.TokenSymbol,
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
                        var input = new CrowdSaleCreateInput(Model);

                        var txResult = await LaunchpadService.CreateAsync(input);

                        if (txResult != null)
                        {
                            if (txResult.ErrorMessage != null)
                                throw new GeneralException(txResult.ErrorMessage.Message);

                            AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Launchpad creation success");
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