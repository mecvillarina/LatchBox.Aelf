using Application.Common.Extensions;
using Blazored.FluentValidation;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.VestingTokenVault;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Vestings.Modals
{
    public partial class AddVestingModal
    {
        [Parameter] public TokenInfo TokenInfo { get; set; } = new();
        [Parameter] public AddVestingParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }
        public List<string> WalletAddresses { get; set; } = new();
        public string TokenBalanceDisplay { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    Model.TokenSymbol = TokenInfo.Symbol;
                    Model.IsRevocable = true;
                    var wallet = await NightElfService.GetAddressAsync();
                    var balanceOutput = await TokenService.GetBalanceAsync(new TokenGetBalanceInput() { Owner = wallet, Symbol = TokenInfo.Symbol });
                    TokenBalanceDisplay = $"{balanceOutput.Balance.ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";
                    IsLoaded = true;
                    StateHasChanged();
                });
            }
        }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                if (Model.Periods.Count < 2)
                {
                    AppDialogService.ShowError($"Vesting MUST have at least 2 periods.");
                }
                else if (Model.Periods.Any(x => x.Receivers.Sum(y => y.Amount) == 0))
                {
                    AppDialogService.ShowError($"Each period MUST have at least 1 receiver.");
                }
                else
                {
                    IsProcessing = true;
                    try
                    {
                        var chain = await ChainService.FetchCurrentChainInfoAsync();
                        var wallet = await NightElfService.GetAddressAsync();
                        var balanceOutput = await TokenService.GetBalanceAsync(new TokenGetBalanceInput() { Owner = wallet, Symbol = TokenInfo.Symbol });

                        if (balanceOutput.Balance.ToAmount(TokenInfo.Decimals) < Convert.ToDecimal(Model.Periods.Sum(x => x.Receivers.Sum(y => y.Amount))))
                            throw new GeneralException($"Insufficient {TokenInfo.Symbol} balance.");

                        var payloadContract = new VestingAddVestingInput(Model, TokenInfo.Decimals);

                        var getAllowanceResult = await TokenService.GetAllowanceAsync(new TokenGetAllowanceInput()
                        {
                            Symbol = Model.TokenSymbol,
                            Owner = wallet,
                            Spender = chain.LockVaultContractAddress
                        });

                        bool isApprove = true;
                        if (getAllowanceResult.Allowance < payloadContract.TotalAmount)
                        {
                            isApprove = false;
                            var approveTxResult = await TokenService.ApproveAsync(new TokenApproveInput()
                            {
                                Spender = chain.VestingVaultContractAddress,
                                Symbol = Model.TokenSymbol,
                                Amount = payloadContract.TotalAmount,
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
                            var txResult = await VestingTokenVaultService.AddVestingAsync(payloadContract);

                            if (txResult != null)
                            {
                                if (txResult.ErrorMessage != null)
                                    throw new GeneralException(txResult.ErrorMessage.Message);

                                AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Add Vesting success");
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
        }

        private async Task InvokeUpsetVestingPeriodModalAsync(UpsetVestingPeriodParameter periodModel = null)
        {
            if (!IsProcessing)
            {
                string title = "Add Vesting Period";
                var parameters = new DialogParameters();
                parameters.Add(nameof(UpsetVestingPeriodModal.TokenInfo), TokenInfo);

                if (periodModel != null)
                {
                    parameters.Add(nameof(UpsetVestingPeriodModal.Model), periodModel);
                    title = "Update Vesting Period";
                }

                var dialog = DialogService.Show<UpsetVestingPeriodModal>(title, parameters);
                var dialogResult = await dialog.Result;

                if (!dialogResult.Cancelled)
                {
                    var period = (UpsetVestingPeriodParameter)dialogResult.Data;

                    if (period.Id == Guid.Empty)
                    {
                        period.Id = Guid.NewGuid();
                        Model.Periods.Add(period);
                    }
                    else
                    {
                        var currentPeriodIndex = Model.Periods.FindIndex(x => x.Id == period.Id);
                        Model.Periods[currentPeriodIndex] = period;
                    }

                    Model.Periods = Model.Periods.OrderBy(x => x.UnlockDate).ToList();
                }
            }
        }


        private void RemovePeriod(Guid id)
        {
            var period = Model.Periods.FirstOrDefault(x => x.Id == id);
            if (period != null)
            {
                Model.Periods.Remove(period);
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}