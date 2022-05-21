using Client.Infrastructure.Extensions;
using Client.Models;
using Client.Pages.Locks.Modals;
using Client.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.Locks
{
    public partial class MyLockClaimsPage
    {
        [Parameter]
        public long? LockId { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }
        public string WalletAddress { get; set; }
        public string ContractLink => $"{BlockchainManager.SideChainExplorer}/address/{LockTokenVaultManager.ContactAddress}";

        public List<LockForReceiverModel> LockTransactions { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    if (LockId.HasValue && LockId.Value > 0)
                    {
                        InvokeLockPreviewerModal(LockId.Value);
                    }

                    await InvokeAsync(async () =>
                    {
                        WalletAddress = await WalletManager.GetWalletAddressAsync();
                        await FetchDataAsync();
                    });
                });
            }
        }


        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            IsCompletelyLoaded = false;
            StateHasChanged();

            LockTransactions.Clear();

            var lockListOutput = await LockTokenVaultManager.GetLocksForReceiverAsync(WalletAddress);

            foreach (var lockTransaction in lockListOutput.LockTransactions)
            {
                LockTransactions.Add(new LockForReceiverModel(lockTransaction.Lock, lockTransaction.Receiver));
            }

            LockTransactions = LockTransactions.OrderByDescending(x => x.Lock.StartTime).ToList();

            IsLoaded = true;
            StateHasChanged();

            foreach (var lockTransaction in LockTransactions)
            {
                var tokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(lockTransaction.Lock.TokenSymbol);
                lockTransaction.SetTokenInfo(tokenInfo);
            }

            IsCompletelyLoaded = true;
            StateHasChanged();
        }

        private async Task InvokeClaimLockModalAsync(LockForReceiverModel lockModel)
        {
            var lockId = lockModel.Lock.LockId;

            var parameters = new DialogParameters();
            parameters.Add(nameof(ClaimLockModal.Model), new ClaimLockParameter()
            {
                LockId = lockId,
                ReceiverAddress = lockModel.Receiver.Receiver.ToStringAddress(),
                AmountDisplay = lockModel.AmountDisplay
            });

            var dialog = DialogService.Show<ClaimLockModal>($"Claim from Lock #{lockId}", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private void InvokeLockPreviewerModal(long lockId)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LockPreviewerModal.LockId), lockId},
            };

            DialogService.Show<LockPreviewerModal>($"Lock #{lockId}", parameters, options);
        }

        private async Task OnTextToClipboardAsync(string text)
        {
            await ClipboardService.WriteTextAsync(text);
            AppDialogService.ShowSuccess("Contract Address copied to clipboard.");
        }

    }
}