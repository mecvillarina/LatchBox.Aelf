using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Launchpads.Modals
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

                    var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (authenticated)
                    {
                        var walletAddress = await WalletManager.GetWalletAddressAsync();
                        var amount = Model.Amount.ToChainAmount(NativeTokenInfo.Decimals);

                        var getAllowanceResult = await TokenManager.GetAllowanceAsync(NativeTokenInfo.Symbol, walletAddress, MultiCrowdSaleManager.ContactAddress);

                        if (getAllowanceResult.Allowance < amount)
                        {
                            await TokenManager.ApproveAsync(MultiCrowdSaleManager.ContactAddress, NativeTokenInfo.Symbol, amount);
                        }

                        var investResult = await MultiCrowdSaleManager.InvestAsync(LaunchpadModel.Launchpad.Id, amount);
                        if (!string.IsNullOrEmpty(investResult.Error))
                            throw new GeneralException(investResult.Error);

                        AppDialogService.ShowSuccess("Launchpad invest success.");
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

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}