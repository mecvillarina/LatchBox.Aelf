﻿using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
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

        private (WalletInformation, string) _cred;
        public List<LockByInitiatorModel> Locks { get; set; } = new();

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

            Locks.Clear();

            var result = await LockTokenVaultManager.InitializeAsync(_cred.Item1, _cred.Item2);
            var lockListOutput = await LockTokenVaultManager.GetLocksByInitiatorAsync(_cred.Item1, _cred.Item2, _cred.Item1.Address);

            foreach (var @lock in lockListOutput.Locks)
            {
                Locks.Add(new LockByInitiatorModel(@lock));
            }

            Locks = Locks.OrderByDescending(x => x.Lock.StartTime).ToList();

            IsLoaded = true;
            StateHasChanged();

            foreach (var @lock in Locks)
            {
                var tokenInfo = await TokenManager.GetTokenInfoAsync(_cred.Item1, _cred.Item2, @lock.Lock.TokenSymbol);
                @lock.SetTokenInfo(tokenInfo);
            }

            IsCompletelyLoaded = true;
            StateHasChanged();
        }

        private async Task InvokeAddLockModalAsync()
        {
            var searchTokenDialog = DialogService.Show<SearchTokenModal>($"Search Token");
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

        private async Task InvokeRevokeLockModalAsync(LockByInitiatorModel lockModel)
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