using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Models;
using Client.Pages.Locks.Modals;
using Client.Parameters;
using MudBlazor;
using System.Numerics;

namespace Client.Pages.Locks
{
    public partial class MyLockClaimsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }

        private (WalletInformation, string) _cred;
        public List<LockForReceiverModel> LockTransactions { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        _cred = await WalletManager.GetWalletCredentialsAsync();
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

            var lockListOutput = await LockTokenVaultManager.GetLocksForReceiverAsync(_cred.Item1, _cred.Item2, _cred.Item1.Address);

            foreach (var lockTransaction in lockListOutput.LockTransactions)
            {
                LockTransactions.Add(new LockForReceiverModel(lockTransaction.Lock, lockTransaction.Receiver));
            }

            LockTransactions = LockTransactions.OrderByDescending(x => x.Lock.StartTime).ToList();

            IsLoaded = true;
            StateHasChanged();

            foreach (var lockTransaction in LockTransactions)
            {
                var tokenInfo = await TokenManager.GetTokenInfoAsync(_cred.Item1, _cred.Item2, lockTransaction.Lock.TokenSymbol);
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

        private void InvokeLockPreviewerModal(BigInteger lockIndex)
        {
            //var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            //var parameters = new DialogParameters()
            //{
            //     { nameof(LockPreviewerModal.LockIndex), lockIndex},
            //};

            //DialogService.Show<LockPreviewerModal>($"Lock #{lockIndex}", parameters, options);
        }

        private async Task OnTextToClipboardAsync(string text)
        {
            await ClipboardService.WriteTextAsync(text);
            AppDialogService.ShowSuccess("Contract Address copied to clipboard.");
        }

    }
}