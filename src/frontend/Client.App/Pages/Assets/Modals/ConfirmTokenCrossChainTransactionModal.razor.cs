using AElf;
using AElf.Types;
using Application.Common.Dtos;
using Blazored.FluentValidation;
using Client.App.Infrastucture.Proto;
using Client.App.Infrastucture.Proto.MultiToken;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Exceptions;
using Google.Protobuf;
using Humanizer;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.App.Pages.Assets.Modals
{
    public partial class ConfirmTokenCrossChainTransactionModal
    {
        [Parameter] public CrossChainPendingOperationDto Model { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsLoaded = true;
                StateHasChanged();
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

                    var input = System.Text.Json.JsonSerializer.Deserialize<TokenValidateInfoExistsInputDto>(Model.Transaction.Transaction.Params, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var @params = new ValidateTokenInfoExistsInput
                    {
                        Symbol = input.Symbol,
                        TokenName = input.TokenName,
                        TotalSupply = input.TotalSupply,
                        Decimals = input.Decimals,
                        Issuer = new Client.App.Infrastucture.Proto.Address { Value = AElf.Types.Address.FromBase58(input.Issuer).Value },
                        IsBurnable = input.IsBurnable,
                        IssueChainId = input.IssueChainId
                    };

                    Transaction transaction = new Transaction()
                    {
                        From = AElf.Types.Address.FromBase58(Model.Transaction.Transaction.From),
                        To = AElf.Types.Address.FromBase58(Model.Transaction.Transaction.To),
                        MethodName = Model.Transaction.Transaction.MethodName,
                        RefBlockNumber = Model.Transaction.Transaction.RefBlockNumber,
                        RefBlockPrefix = ByteString.FromBase64(Model.Transaction.Transaction.RefBlockPrefix),
                        Params = @params.ToByteString(),
                        Signature = ByteString.FromBase64(Model.Transaction.Transaction.Signature)
                    };

                    var payloadContract = new TokenCrossChainCreateInput()
                    {
                        FromChainId = Model.FromChainId,
                        ParentChainHeight = Model.Transaction.BlockNumber,
                        TransactionBytes = transaction.ToByteString().ToBase64(),
                        MerklePath = Model.MerklePath
                    };

                    var txResult = await TokenService.CrossChainCreateTokenAsync(payloadContract);

                    if (txResult != null)
                    {
                        if (txResult.ErrorMessage != null)
                            throw new GeneralException(txResult.ErrorMessage.Message);

                        AppDialogService.ShowTxSend(chain.Explorer, txResult.TransactionId, "Transaction confirmed");
                        MudDialog.Close();
                    }
                }
                catch (Exception ex)
                {
                    AppDialogService.ShowError(ex.Message);
                    MudDialog.Cancel();
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