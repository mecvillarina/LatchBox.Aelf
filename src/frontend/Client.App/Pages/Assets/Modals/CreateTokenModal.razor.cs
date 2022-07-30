using Application.Common.Extensions;
using Application.Features.CrossChainOperations.Commands.Create;
using Blazored.FluentValidation;
using Client.App.Models;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Assets.Modals
{
    public partial class CreateTokenModal
    {
        [Parameter] public CreateTokenParameter Model { get; set; } = new CreateTokenParameter(true, 0);
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public List<SupportedChainModel> SupportedChains { get; set; } = new List<SupportedChainModel>();

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    await FetchDataAsync();
                    StateHasChanged();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            var chains = await ChainService.FetchSupportedChainsAsync();
            SupportedChains = chains.Select(x => new SupportedChainModel(x)).ToList();

            if (SupportedChains.Any())
            {
                var issueChainId = SupportedChains.First().ChainId;
                Model = new CreateTokenParameter(true, issueChainId);
            }
        }
        private async Task SubmitAsync()
        {
            if (Validated)
            {
                IsProcessing = true;

                try
                {
                    var chain = await ChainService.FetchCurrentChainInfoAsync();
                    var walletAddress = await NightElfService.GetAddressAsync();

                    var payloadContract = new TokenCreateTokenInput(Model) { Issuer = walletAddress, TotalSupply = Model.TotalSupply.ToChainAmount(Model.Decimals) };
                    var txResult = await TokenService.CreateTokenAsync(payloadContract);

                    if (txResult != null)
                    {
                        if (txResult.ErrorMessage != null)
                            throw new GeneralException(txResult.ErrorMessage.Message);

                        var validateInfoPayload = new TokenValidateInfoExistsInput(Model) { Issuer = walletAddress, TotalSupply = Model.TotalSupply.ToChainAmount(Model.Decimals) };

                        var validateInfoTxResult = await TokenService.ValidateTokenInfoExistsAsync(validateInfoPayload);

                        if (validateInfoTxResult != null)
                        {
                            if (validateInfoTxResult.ErrorMessage != null)
                                throw new GeneralException(validateInfoTxResult.ErrorMessage.Message);

                            if (chain.ChainId != Model.IssueChainId)
                            {
                                await _exceptionHandler.HandlerRequestTaskAsync(() => CrossChainOperationManager.CreateAsync(new CreateCrossChainOperationCommand()
                                {
                                    From = walletAddress,
                                    ChainId = chain.ChainId,
                                    IssueChainId = Model.IssueChainId,
                                    ContactName = "Token",
                                    ChainOperation = "Create",
                                    TransactionId = validateInfoTxResult.TransactionId
                                }));

                                var issueChain = SupportedChains.FirstOrDefault(x => x.ChainId == Model.IssueChainId);
                                AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, $"Token creation success on {chain.ChainIdBase58} chain. After few minutes, please change your current chain to {issueChain.ChainIdBase58} chain as there is another transaction needed to be confirm.");
                            }
                            else
                            {
                                AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Token creation success");
                            }
                        }

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