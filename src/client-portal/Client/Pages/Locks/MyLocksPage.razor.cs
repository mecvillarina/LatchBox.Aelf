using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Models;
using Client.Pages.Locks.Modals;
using Client.Pages.Modals;
using Client.Parameters;
using MudBlazor;

namespace Client.Pages.Locks
{
    public partial class MyLocksPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }
        public string WalletAddress { get; set; }
        public string ContractLink => $"{BlockchainManager.SideChainExplorer}/address/{LockTokenVaultManager.ContactAddress}";
        public string SideChain { get; set; }
        public List<LockModel> Locks { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        WalletAddress = await WalletManager.GetWalletAddressAsync();
                        SideChain = $"Side {BlockchainManager.GetSideChainId().ToStringChainId()}";

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

            Locks.Clear();

            var lockListOutput = await LockTokenVaultManager.GetLocksByInitiatorAsync(WalletAddress);

            foreach (var @lock in lockListOutput.Locks)
            {
                Locks.Add(new LockModel(@lock));
            }

            Locks = Locks.OrderByDescending(x => x.Lock.StartTime).ToList();

            IsLoaded = true;
            StateHasChanged();

            try
            {
                var tasks = new List<Task>();

                foreach (var @lock in Locks)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(@lock.Lock.TokenSymbol);
                        @lock.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);

                IsCompletelyLoaded = true;
                StateHasChanged();
            }
            catch
            {

            }
        }

        private async Task InvokeAddLockModalAsync()
        {
            var searchTokenDialog = DialogService.Show<SearchTokenModal>($"Search Token ({SideChain} Chain)");
            var searchTokenDialogResult = await searchTokenDialog.Result;

            if (!searchTokenDialogResult.Cancelled)
            {
                var tokenInfo = (TokenInfo)searchTokenDialogResult.Data;

                var parameters = new DialogParameters();
                parameters.Add(nameof(AddLockModal.TokenInfo), tokenInfo);

                var dialog = DialogService.Show<AddLockModal>($"Add New Lock", parameters);
                var dialogResult = await dialog.Result;

                if (!dialogResult.Cancelled)
                {
                    await FetchDataAsync();
                }
            }
        }

        private async Task InvokeRevokeLockModalAsync(LockModel lockModel)
        {
            var lockId = lockModel.Lock.LockId;

            var parameters = new DialogParameters();
            parameters.Add(nameof(RevokeLockModal.Lock), lockModel.Lock);
            parameters.Add(nameof(RevokeLockModal.Model), new RevokeLockParameter() { LockId = lockId });

            var dialog = DialogService.Show<RevokeLockModal>($"Revoke Lock #{lockId}", parameters);
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

    }
}