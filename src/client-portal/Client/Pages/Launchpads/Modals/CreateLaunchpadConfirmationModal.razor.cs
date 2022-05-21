using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models.Inputs;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Launchpads.Modals
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

                    var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (authenticated)
                    {
                        var walletAddress = await WalletManager.GetWalletAddressAsync();

                        var amount = (long)(Model.TokenAmountPerNativeToken.ToChainAmount(Model.TokenDecimals) * Model.HardCapNativeTokenAmount);

                        var getAllowanceResult = await TokenManager.GetAllowanceAsync(Model.TokenSymbol, walletAddress, MultiCrowdSaleManager.ContactAddress);

                        if (getAllowanceResult.Allowance < amount)
                        {
                            await TokenManager.ApproveAsync(MultiCrowdSaleManager.ContactAddress, Model.TokenSymbol, amount);
                        }

                        var saleStartDate = DateTime.SpecifyKind(Model.SaleStartDate.Value.Date, DateTimeKind.Utc);
                        var saleEndDate = DateTime.SpecifyKind(Model.SaleEndDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc);

                        var input = new CreateLaunchpadInputModel()
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

                        var createResult = await MultiCrowdSaleManager.CreateAsync(input);

                        if (!string.IsNullOrEmpty(createResult.Error))
                            throw new GeneralException(createResult.Error);

                        AppDialogService.ShowSuccess("Launchpad creation success.");
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