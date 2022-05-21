using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models.Inputs;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Vestings.Modals
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

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Model.TokenSymbol = TokenInfo.Symbol;
                Model.IsRevocable = true;
                IsLoaded = true;
                StateHasChanged();
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
                        var token = await TokenManager.GetBalanceOnSideChainAsync(TokenInfo.Symbol);

                        if (token.Balance.ToAmount(TokenInfo.Decimals) < Convert.ToDecimal(Model.Periods.Sum(x => x.Receivers.Sum(y => y.Amount))))
                            throw new GeneralException($"Insufficient {TokenInfo.Symbol} balance.");

                        var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                        if (authenticated)
                        {
                            var walletAddress = await WalletManager.GetWalletAddressAsync();

                            long totalAmount = 0;

                            var inputPeriods = new List<AddVestingPeriodInputModel>();

                            foreach (var periodModel in Model.Periods)
                            {
                                var period = new AddVestingPeriodInputModel();
                                period.Name = periodModel.Name;
                                period.UnlockTime = DateTime.SpecifyKind(periodModel.UnlockDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc);
                                period.Receivers = new List<AddVestingReceiverInputModel>();

                                foreach (var receiverModel in periodModel.Receivers)
                                {
                                    var amount = receiverModel.Amount.ToChainAmount(TokenInfo.Decimals);

                                    period.Receivers.Add(new AddVestingReceiverInputModel()
                                    {
                                        Name = receiverModel.Name,
                                        Address = receiverModel.ReceiverAddress,
                                        Amount = amount
                                    });

                                    period.TotalAmount += amount;
                                }

                                inputPeriods.Add(period);

                                totalAmount += period.TotalAmount;
                            }

                            var inputModel = new AddVestingInputModel()
                            {
                                TokenSymbol = Model.TokenSymbol,
                                TotalAmount = totalAmount,
                                IsRevocable = Model.IsRevocable,
                                Periods = inputPeriods
                            };

                            var getAllowanceResult = await TokenManager.GetAllowanceAsync(Model.TokenSymbol, walletAddress, VestingTokenVaultManager.ContactAddress);

                            if (getAllowanceResult.Allowance < totalAmount)
                            {
                                await TokenManager.ApproveAsync(VestingTokenVaultManager.ContactAddress, Model.TokenSymbol, totalAmount);
                            }

                            var addVestingResult = await VestingTokenVaultManager.AddVestingAsync(inputModel);

                            if (!string.IsNullOrEmpty(addVestingResult.Error))
                                throw new GeneralException(addVestingResult.Error);

                            AppDialogService.ShowSuccess("Add Vesting success.");
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