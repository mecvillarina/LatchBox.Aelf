﻿using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.CrowdFundings.Modals
{
    public partial class InvestOnCrowdSaleConfirmationModal
    {
        [Parameter] public InvestOnCrowdSaleParameter Model { get; set; }
        [Parameter] public CrowdSaleModel CrowdSaleModel { get; set; }
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

                    var cred = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (cred.Item1 != null)
                    {
                        var amount = Model.Amount.ToChainAmount(NativeTokenInfo.Decimals);

                        var getAllowanceResult = await TokenManager.GetAllowanceAsync(cred.Item1, cred.Item2, NativeTokenInfo.Symbol, cred.Item1.Address, MultiCrowdSaleManager.ContactAddress);

                        if (getAllowanceResult.Allowance < amount)
                        {
                            await TokenManager.ApproveAsync(cred.Item1, cred.Item2, MultiCrowdSaleManager.ContactAddress, NativeTokenInfo.Symbol, amount);
                        }

                        var investOnCrowdSaleResult = await MultiCrowdSaleManager.InvestAsync(cred.Item1, cred.Item2, CrowdSaleModel.CrowdSale.Id, amount);
                        if (!string.IsNullOrEmpty(investOnCrowdSaleResult.Error))
                        {
                            throw new GeneralException(investOnCrowdSaleResult.Error);
                        }
                        else
                        {
                            AppDialogService.ShowSuccess("Launchpad invest success.");
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