using AElf;
using AElf.Client.LatchBox.LockTokenVault;
using AElf.Client.LatchBox.MultiCrowdSale;
using AElf.Client.MultiToken;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Infrastructure.Models.Inputs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Launchpads.Modals
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

                var authenticated = await AppDialogService.ShowConfirmWalletTransactionAsync();

                if (authenticated)
                {
                    var txResult = await MultiCrowdSaleManager.CompleteAsync(Model.Launchpad.Id);

                    if (!string.IsNullOrEmpty(txResult.Error))
                        throw new GeneralException(txResult.Error);

                    var result = ResultOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(txResult.ReturnValue));

                    if (!result.IsSuccess)
                        throw new GeneralException(result.Message);

                    var wallet = await WalletManager.GetWalletInformationAsync();

                    var crowdSaleOutput = await MultiCrowdSaleManager.GetCrowdSaleAsync(Model.Launchpad.Id);
                    var investmentsOutput = await MultiCrowdSaleManager.GetCrowdSaleInvestments(Model.Launchpad.Id);
                    var nativeTokenInfo = await TokenManager.GetNativeTokenInfoAsync();

                    long totalAmount = 0;
                    var inputReceivers = new List<AddLockReceiverInputModel>();

                    foreach (var investment in investmentsOutput.List)
                    {
                        var amount = crowdSaleOutput.CrowdSale.TokenAmountPerNativeToken * investment.TokenAmount.ToAmount(nativeTokenInfo.Decimals);
                        var chainAmount = Convert.ToInt64(amount);
                        totalAmount += chainAmount;

                        inputReceivers.Add(new AddLockReceiverInputModel() { ReceiverAddress = investment.Investor.ToStringAddress(), Amount = chainAmount });
                    }

                    var unlockTime = DateTime.UtcNow.AddMinutes(crowdSaleOutput.CrowdSale.LockUntilDurationInMinutes);

                    var inputModel = new AddLockInputModel()
                    {
                        TokenSymbol = Model.TokenSymbol,
                        TotalAmount = totalAmount,
                        IsRevocable = false,
                        Receivers = inputReceivers,
                        UnlockTime = unlockTime,
                        Remarks = $"LAUNCHPAD: {crowdSaleOutput.CrowdSale.Name}"
                    };

                    var getAllowanceResult = await TokenManager.GetAllowanceAsync(Model.TokenSymbol, wallet.Address, LockTokenVaultManager.ContactAddress);

                    if (getAllowanceResult.Allowance < totalAmount)
                    {
                        await TokenManager.ApproveAsync(LockTokenVaultManager.ContactAddress, Model.TokenSymbol, totalAmount);
                    }

                    var addLockTxResult = await LockTokenVaultManager.AddLockAsync(inputModel);

                    if (!string.IsNullOrEmpty(addLockTxResult.Error))
                        throw new GeneralException(addLockTxResult.Error);

                    var addLockOutput = AddLockOuput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(addLockTxResult.ReturnValue));

                    var updateLockInfoTxResult = await MultiCrowdSaleManager.UpdateLockInfoAsync(Model.Launchpad.Id, addLockOutput.LockId);

                    if (!string.IsNullOrEmpty(updateLockInfoTxResult.Error))
                        throw new GeneralException(updateLockInfoTxResult.Error);

                    AppDialogService.ShowSuccess("Launchpad completion success.");
                    MudDialog.Close();
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