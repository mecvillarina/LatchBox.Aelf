using AElf;
using AElf.Client.Proto;
using Blazored.FluentValidation;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Tokens.Modals
{
    public partial class AddExistingTokenModal
    {
        [Parameter] public SearchTokenParameter Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public string MainChainId { get; set; }
        public string SideChainId { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    MainChainId = $"Main {(await BlockchainManager.GetMainChainIdAsync()).ToStringChainId()}";
                    SideChainId = $"Side {(await BlockchainManager.GetSideChainIdAsync()).ToStringChainId()}";
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
                    var sideChainTokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(Model.Symbol.ToUpper());

                    if (!string.IsNullOrEmpty(sideChainTokenInfo.Symbol))
                    {
                        await TokenManager.AddTokenSymbolToStorageAsync(Model.Symbol.ToUpper());
                        MudDialog.Close();
                    }
                    else
                    {
                        var tokenInfo = await TokenManager.GetTokenInfoOnMainChainAsync(Model.Symbol.ToUpper());
                        var walletAddress = await WalletManager.GetWalletAddressAsync();

                        if (!string.IsNullOrEmpty(tokenInfo.Symbol) && walletAddress == tokenInfo.Issuer.ToStringAddress())
                        {
                            var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                            if (authenticated)
                            {
                                var createSideChainTokenResult = await TokenManager.CreateSideChainTokenAsync(tokenInfo);

                                if (!string.IsNullOrEmpty(createSideChainTokenResult.Error))
                                    throw new GeneralException(createSideChainTokenResult.Error);

                                await TokenManager.AddTokenSymbolToStorageAsync(Model.Symbol.ToUpper());
                                MudDialog.Close();
                            }
                        }
                        else
                        {
                            AppDialogService.ShowError("Token not found.");
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