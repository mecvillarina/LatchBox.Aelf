using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Numerics;

namespace Client.Pages.Locks.Modals
{
    public partial class AssetLockPreviewerModal
    {
        [Parameter] public AssetCounterModel AssetCounterModel { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        public bool IsLoaded { get; set; }
        private (WalletInformation, string) _cred;

        public TokenInfo TokenInfo { get; set; }
        public List<LockModel> Locks { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    TokenInfo = AssetCounterModel.TokenInfo;
                    _cred = await WalletManager.GetWalletCredentialsAsync();
                    await FetchDataAsync();
                });
            }
        }


        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            Locks = new();

            var lockListOutput = await LockTokenVaultManager.GetLocksByAssetAsync(_cred.Item1, _cred.Item2, TokenInfo.Symbol);

            foreach (var @lock in lockListOutput.Locks)
            {
                var lockModel = new LockModel(@lock);
                lockModel.SetTokenInfo(TokenInfo);
                Locks.Add(lockModel);
            }

            IsLoaded = true;
            StateHasChanged();
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