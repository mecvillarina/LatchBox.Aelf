using AElf;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Tokens.Modals
{
    public partial class IssueTokenModal
    {
        [Parameter] public IssueTokenParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public int MainChainId { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(() =>
                {
                    MainChainId = BlockchainManager.GetMainChainId();
                    StateHasChanged();
                });
            }
        }

        private async Task SubmitAsync()
        {
            if (Validated)
            {
                try
                {
                    if (Model.Amount.CountDecimalPlaces() > Model.Decimals)
                    {
                        if (Model.Decimals > 0)
                        {
                            AppDialogService.ShowError($"Allowable decimal places for 'Amount' less than or equal to {Model.Decimals}");
                        }
                        else
                        {
                            AppDialogService.ShowError($"{Model.TokenName} doesn't allow 'Amount' to have decimal places.");
                        }

                        return;
                    }

                    IsProcessing = true;

                    var amount = Model.Amount.ToChainAmount(Model.Decimals);
                    var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (authenticated)
                    {
                        if (Model.IssuedChainId == MainChainId)
                        {
                            var issueTokenResult = await TokenManager.IssueOnMainChainAsync(Model.Symbol, amount, Model.Memo, Model.To);

                            if (!string.IsNullOrEmpty(issueTokenResult.Error))
                                throw new GeneralException(issueTokenResult.Error);
                        }
                        else
                        {
                            var issueTokenResult = await TokenManager.IssueOnSideChainAsync(Model.Symbol, amount, Model.Memo, Model.To);

                            if (!string.IsNullOrEmpty(issueTokenResult.Error))
                                throw new GeneralException(issueTokenResult.Error);
                        }

                        AppDialogService.ShowSuccess("Issue new token success.");
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

        private async Task SetToValueAsync()
        {
            var walletAddress = await WalletManager.GetWalletAddressAsync();
            Model.To = walletAddress;
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}