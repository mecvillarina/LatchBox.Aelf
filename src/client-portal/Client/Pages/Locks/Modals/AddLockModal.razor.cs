using AElf.Client.MultiToken;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models.Inputs;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks.Modals
{
    public partial class AddLockModal
    {
        [Parameter] public TokenInfo TokenInfo { get; set; } = new();
        [Parameter] public AddLockParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }
        public DateTime MinDateValue { get; set; }
        public string TokenBalanceDisplay { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    Model.TokenSymbol = TokenInfo.Symbol;
                    MinDateValue = DateTime.Now;
                    Model.UnlockDate = MinDateValue.AddDays(1);
                    Model.IsRevocable = true;
                    Model.Remarks = "";
                    var balanceOutput = await TokenManager.GetBalanceAsync(TokenInfo.Symbol);
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
                if (!Model.Receivers.Any())
                {
                    AppDialogService.ShowError($"Please add receivers for your lock.");
                }
                else
                {
                    IsProcessing = true;

                    try
                    {
                        var token = await TokenManager.GetBalanceAsync(TokenInfo.Symbol);

                        if (token.Balance.ToAmount(TokenInfo.Decimals) < Convert.ToDecimal(Model.Receivers.Sum(x => x.Amount)))
                            throw new GeneralException($"Insufficient {TokenInfo.Symbol} balance.");

                        var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                        if (authenticated)
                        {
                            var wallet = await WalletManager.GetWalletInformationAsync();

                            long totalAmount = 0;

                            var inputReceivers = new List<AddLockReceiverInputModel>();

                            foreach (var receiver in Model.Receivers)
                            {
                                var amount = receiver.Amount.ToChainAmount(TokenInfo.Decimals);
                                inputReceivers.Add(new AddLockReceiverInputModel()
                                {
                                    ReceiverAddress = receiver.ReceiverAddress,
                                    Amount = amount
                                });

                                totalAmount += amount;
                            }

                            var unlockTime = DateTime.SpecifyKind(Model.UnlockDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc);

                            var inputModel = new AddLockInputModel()
                            {
                                TokenSymbol = Model.TokenSymbol,
                                TotalAmount = totalAmount,
                                IsRevocable = Model.IsRevocable,
                                Receivers = inputReceivers,
                                UnlockTime = unlockTime,
                                Remarks = Model.Remarks
                            };

                            var getAllowanceResult = await TokenManager.GetAllowanceAsync(Model.TokenSymbol, wallet.Address, LockTokenVaultManager.ContactAddress);

                            if (getAllowanceResult.Allowance < totalAmount)
                            {
                                await TokenManager.ApproveAsync(LockTokenVaultManager.ContactAddress, Model.TokenSymbol, totalAmount);
                            }

                            var addLockResult = await LockTokenVaultManager.AddLockAsync(inputModel);
                            if (!string.IsNullOrEmpty(addLockResult.Error))
                                throw new GeneralException(addLockResult.Error);

                            AppDialogService.ShowSuccess("Add Lock success.");
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

        private async Task InvokeAddLockReceiverModalAsync()
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(AddLockReceiverModal.TokenInfo), TokenInfo);

            var dialog = DialogService.Show<AddLockReceiverModal>($"Add Lock Receiver", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                var receiver = (AddLockReceiverParameter)dialogResult.Data;

                if (Model.Receivers.Any(x => x.ReceiverAddress == receiver.ReceiverAddress))
                {
                    var currentReceiver = Model.Receivers.First(x => x.ReceiverAddress == receiver.ReceiverAddress);
                    currentReceiver.Amount += receiver.Amount;
                }
                else
                {
                    Model.Receivers.Add(new AddLockReceiverParameter()
                    {
                        Id = Guid.NewGuid(),
                        ReceiverAddress = receiver.ReceiverAddress,
                        Amount = receiver.Amount
                    });
                }
            }
        }

        private void RemoveReceiver(Guid id)
        {
            var receiver = Model.Receivers.FirstOrDefault(x => x.Id == id);
            if (receiver != null)
            {
                Model.Receivers.Remove(receiver);
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}