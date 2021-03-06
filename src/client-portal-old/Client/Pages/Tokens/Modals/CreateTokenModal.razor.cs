using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Tokens.Modals
{
    public partial class CreateTokenModal
    {
        [Parameter] public CreateTokenParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public string MainChainId { get; set; }
        public string SideChainId { get; set; }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    MainChainId = $"Main {BlockchainManager.GetMainChainId().ToChainName()}";
                    SideChainId = $"Side {BlockchainManager.GetSideChainId().ToChainName()}";
                    StateHasChanged();
                });
            }
        }
        private async Task SubmitAsync()
        {
            if (Validated)
            {
                IsProcessing = true;

                try
                {
                    var totalSupply = Model.TotalSupply.ToChainAmount(Model.Decimals);
                    //var initialSupply = Model.InitialSupply.ToChainAmount(Model.Decimals);
                    var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                    if (authenticated)
                    {
                        //var walletAddress = await WalletManager.GetWalletAddressAsync();
                        var createTokenResult = await TokenManager.CreateAsync(Model.Symbol.ToUpper(), Model.TokenName, totalSupply, Model.Decimals, Model.IsBurnable);

                        if (!string.IsNullOrEmpty(createTokenResult.Error))
                            throw new GeneralException(createTokenResult.Error);

                        //if (initialSupply > 0)
                        //{
                        //    var issueTokenResult = await TokenManager.IssueOnMainChainAsync(Model.Symbol, initialSupply, "Initial Supply", walletAddress);

                        //    if (!string.IsNullOrEmpty(issueTokenResult.Error))
                        //        throw new GeneralException(issueTokenResult.Error);
                        //}
                        await BlockchainManager.GetMainChainStatusAsync();
                        await BlockchainManager.GetSideChainStatusAsync();
                        await TokenManager.AddToTokenSymbolsStorageAsync(Model.Symbol.ToUpper());
                        AppDialogService.ShowSuccess("Token creation success.");
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