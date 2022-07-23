using Application.Common.Extensions;
using Blazored.FluentValidation;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Locks.Modals
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
                if (!Model.Receivers.Any())
                {
                    AppDialogService.ShowError($"Please add receivers for your lock.");
                }
                else
                {
                    IsProcessing = true;

                    try
                    {
                        var chain = await ChainService.FetchCurrentChainInfoAsync();
                        var wallet = await NightElfService.GetAddressAsync();
                        var balanceOutput = await TokenService.GetBalanceAsync(new TokenGetBalanceInput() { Owner = wallet, Symbol = TokenInfo.Symbol });

                        if (balanceOutput.Balance.ToAmount(TokenInfo.Decimals) < Convert.ToDecimal(Model.Receivers.Sum(x => x.Amount)))
                            throw new GeneralException($"Insufficient {TokenInfo.Symbol} balance.");

                        var payloadContract = new LockAddLockInput(Model, TokenInfo.Decimals);

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
                                Spender = chain.LockVaultContractAddress,
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
                            var txResult = await LockTokenVaultService.AddLockAsync(payloadContract);

                            if (txResult != null)
                            {
                                if (txResult.ErrorMessage != null)
                                    throw new GeneralException(txResult.ErrorMessage.Message);

                                AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Add Lock success");
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