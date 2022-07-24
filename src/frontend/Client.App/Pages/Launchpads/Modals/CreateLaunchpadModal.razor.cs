using Application.Common.Extensions;
using Blazored.FluentValidation;
using Client.App.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads.Modals
{
    public partial class CreateLaunchpadModal
    {
        [Parameter] public CreateLaunchpadParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }
        public DateTime MinDateValue { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                MinDateValue = DateTime.Now;
                Model.SaleStartDate = MinDateValue.AddDays(1);
                Model.SaleEndDate = MinDateValue.AddDays(1);
                StateHasChanged();
            }
        }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                if (Model.SoftCapNativeTokenAmount.CountDecimalPlaces() > Model.NativeTokenDecimals)
                {
                    if (Model.NativeTokenDecimals > 0)
                    {
                        AppDialogService.ShowError($"Allowable decimal places for 'Soft Cap' less than or equal to {Model.NativeTokenDecimals}");
                    }
                    else
                    {
                        AppDialogService.ShowError($"{Model.NativeTokenName} doesn't allow 'Soft Cap' to have decimal places.");
                    }

                    return;
                }
                else if (Model.HardCapNativeTokenAmount.CountDecimalPlaces() > Model.NativeTokenDecimals)
                {
                    if (Model.NativeTokenDecimals > 0)
                    {
                        AppDialogService.ShowError($"Allowable decimal places for 'Hard Cap' less than or equal to {Model.NativeTokenDecimals}");
                    }
                    else
                    {
                        AppDialogService.ShowError($"{Model.NativeTokenName} doesn't allow 'Hard Cap' to have decimal places.");
                    }

                    return;
                }
                else if (Model.TokenAmountPerNativeToken.CountDecimalPlaces() > Model.TokenDecimals)
                {
                    if (Model.TokenDecimals > 0)
                    {
                        AppDialogService.ShowError($"Allowable decimal places for 'Token Amount' less than or equal to {Model.TokenDecimals}");
                    }
                    else
                    {
                        AppDialogService.ShowError($"{Model.TokenName} doesn't allow 'Token Amount' to have decimal places.");
                    }

                    return;
                }
                else if (Model.NativeTokenPurchaseLimitPerBuyerAddress.CountDecimalPlaces() > Model.NativeTokenDecimals)
                {
                    if (Model.NativeTokenDecimals > 0)
                    {
                        AppDialogService.ShowError($"Allowable decimal places for 'Token Purchase limit per buyer address' less than or equal to {Model.NativeTokenDecimals}");
                    }
                    else
                    {
                        AppDialogService.ShowError($"{Model.NativeTokenName} doesn't allow 'Token Purchase limit per buyer address' to have decimal places.");
                    }

                    return;
                }

                var confirmationParameters = new DialogParameters()
                    {
                        { nameof(CreateLaunchpadConfirmationModal.Model), Model}
                    };

                var createDialog = DialogService.Show<CreateLaunchpadConfirmationModal>("Create Launchpad Confirmation", confirmationParameters);
                var createDialogResult = await createDialog.Result;

                if (!createDialogResult.Cancelled)
                {
                    MudDialog.Close();
                }
            }
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}