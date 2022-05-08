using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models.Inputs;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.CrowdFundings.Modals
{
    public partial class CreateCrowdSaleConfirmationModal
    {
        [Parameter] public CreateCrowdSaleParameter Model { get; set; } = new();
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

                    var amount = (long)(Model.TokenAmountPerNativeToken.ToChainAmount(Model.TokenDecimals) * Model.HardCapNativeTokenAmount);
                    var cred = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (cred.Item1 != null)
                    {
                        var getAllowanceResult = await TokenManager.GetAllowanceAsync(cred.Item1, cred.Item2, Model.TokenSymbol, cred.Item1.Address, MultiCrowdSaleManager.ContactAddress);

                        if(getAllowanceResult.Allowance < amount)
                        {
                            await TokenManager.ApproveAsync(cred.Item1, cred.Item2, MultiCrowdSaleManager.ContactAddress, Model.TokenSymbol, amount);
                        }

                        var saleStartDate = DateTime.SpecifyKind(Model.SaleEndDate.Value.Date, DateTimeKind.Utc);
                        var saleEndDate = DateTime.SpecifyKind(Model.SaleEndDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc);

                        var input = new CreateCrowdSaleInputModel()
                        {
                            Name = Model.Name,
                            TokenSymbol = Model.TokenSymbol,
                            SoftCapNativeTokenAmount = Model.SoftCapNativeTokenAmount.ToChainAmount(Model.NativeTokenDecimals),
                            HardCapNativeTokenAmount = Model.HardCapNativeTokenAmount.ToChainAmount(Model.NativeTokenDecimals),
                            NativeTokenPurchaseLimitPerBuyerAddress = Model.NativeTokenPurchaseLimitPerBuyerAddress.ToChainAmount(Model.NativeTokenDecimals),
                            TokenAmountPerNativeToken = Model.TokenAmountPerNativeToken.ToChainAmount(Model.TokenDecimals),
                            LockUntilDurationInMinutes = Model.LockUntilDurationInMinutes,
                            SaleStartDate = saleStartDate,
                            SaleEndDate = saleEndDate
                        };

                        var createCrowdSaleResult = await MultiCrowdSaleManager.CreateAsync(cred.Item1, cred.Item2, input);

                        if (!string.IsNullOrEmpty(createCrowdSaleResult.Error))
                        {
                            throw new GeneralException(createCrowdSaleResult.Error);
                        }
                        else
                        {
                            AppDialogService.ShowSuccess("Launchpad creation success.");
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