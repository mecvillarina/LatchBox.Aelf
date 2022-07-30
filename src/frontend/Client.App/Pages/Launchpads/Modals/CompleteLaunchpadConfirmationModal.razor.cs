using AElf;
using Application.Common.Extensions;
using Client.App.Infrastucture.Proto.Launchpad;
using Client.App.Infrastucture.Proto.LockTokenVault;
using Client.App.Models;
using Client.App.Services;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.Launchpad;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Exceptions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads.Modals
{
    public partial class CompleteLaunchpadConfirmationModal
    {
        [Parameter] public MyLaunchpadModel Model { get; set; }
        [Parameter] public TokenInfo NativeTokenInfo { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsProcessing { get; set; }

        private async Task SubmitAsync()
        {
            try
            {
                IsProcessing = true;

                var txResult = await LaunchpadService.CompleteAsync(new CrowdSaleCompleteInput()
                {
                    CrowdSaleId = Model.Launchpad.Id
                });

                if (txResult != null)
                {
                    if (txResult.ErrorMessage != null)
                        throw new GeneralException(txResult.ErrorMessage.Message);

                    var result = ResultOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(txResult.ReturnValue));

                    if (!result.IsSuccess)
                        throw new GeneralException(result.Message);

                    var chain = await ChainService.FetchCurrentChainInfoAsync();
                    var walletAddress = await NightElfService.GetAddressAsync();

                    var crowdSaleOutput = await LaunchpadService.GetCrowdSaleAsync(new CrowdSaleGetCrowdSaleInput()
                    {
                        CrowdSaleId = Model.Launchpad.Id
                    });
                    var investmentsOutput = await LaunchpadService.GetCrowdSaleInvestmentsAsync(new CrowdSaleGetCrowdSaleInvestorsInput()
                    {
                        CrowdSaleId = Model.Launchpad.Id
                    });

                    var nativeTokenInfo = await TokenService.GetNativeTokenInfoAsync();

                    long totalAmount = 0;
                    var inputReceivers = new List<LockAddLockReceiverInput>();

                    foreach (var investment in investmentsOutput.List)
                    {
                        var amount = crowdSaleOutput.CrowdSale.TokenAmountPerNativeToken * investment.TokenAmount.ToAmount(nativeTokenInfo.Decimals);
                        var chainAmount = Convert.ToInt64(amount);
                        totalAmount += chainAmount;

                        inputReceivers.Add(new LockAddLockReceiverInput() { Receiver = investment.Investor, Amount = chainAmount });
                    }

                    var payloadContract = new LockAddLockInput()
                    {
                        TokenSymbol = Model.TokenSymbol,
                        IsRevocable = false,
                        Remarks = $"LAUNCHPAD: {crowdSaleOutput.CrowdSale.Name}",
                        UnlockTime = Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(crowdSaleOutput.CrowdSale.LockUntilDurationInMinutes)),
                        Receivers = inputReceivers,
                        TotalAmount = totalAmount
                    };

                    var getAllowanceResult = await TokenService.GetAllowanceAsync(new TokenGetAllowanceInput()
                    {
                        Symbol = Model.TokenSymbol,
                        Owner = walletAddress,
                        Spender = chain.LockVaultContractAddress
                    });

                    var isApprove = true;
                    if (getAllowanceResult.Allowance < totalAmount)
                    {
                        isApprove = false;
                        var approveTxResult = await TokenService.ApproveAsync(new TokenApproveInput()
                        {
                            Spender = chain.LockVaultContractAddress,
                            Symbol = Model.TokenSymbol,
                            Amount = totalAmount,
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
                        var addLockTxResult = await LockTokenVaultService.AddLockAsync(payloadContract);

                        if (addLockTxResult != null)
                        {
                            if (addLockTxResult.ErrorMessage != null)
                                throw new GeneralException(addLockTxResult.ErrorMessage.Message);

                            var addLockOutput = AddLockOuput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(addLockTxResult.ReturnValue));

                            var updateLockInfoTxResult = await LaunchpadService.UpdateLockInfoAsync(new CrowdSaleUpdateLockInfoInput()
                            {
                                CrowdSaleId = Model.Launchpad.Id,
                                LockId = addLockOutput.LockId
                            });

                            if (updateLockInfoTxResult != null)
                            {
                                if (updateLockInfoTxResult.ErrorMessage != null)
                                    throw new GeneralException(updateLockInfoTxResult.ErrorMessage.Message);

                                AppDialogService.ShowSuccess("Launchpad completion success");
                                MudDialog.Close();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                AppDialogService.ShowError(ex.Message);
                MudDialog.Close();
            }

            IsProcessing = false;
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}