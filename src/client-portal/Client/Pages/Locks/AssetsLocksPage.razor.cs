using Client.Infrastructure.Models;
using Client.Models;
using Client.Pages.Locks.Modals;
using MudBlazor;

namespace Client.Pages.Locks
{
    public partial class AssetsLocksPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }
        public string ContractLink => $"{BlockchainManager.SideChainExplorer}/address/{LockTokenVaultManager.ContactAddress}";

        public List<AssetCounterModel> AssetCounters { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;
                    await InvokeAsync(async () =>
                    {
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
            AssetCounters.Clear();

            var assetsCounterOutput = await LockTokenVaultManager.GetAssetsCounterAsync();

            foreach (var assetCounter in assetsCounterOutput.Assets)
            {
                AssetCounters.Add(new AssetCounterModel(assetCounter));
            }

            IsLoaded = true;
            StateHasChanged();

            foreach (var assetCounter in AssetCounters)
            {
                var tokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(assetCounter.TokenSymbol);
                assetCounter.SetTokenInfo(tokenInfo);
            }

            AssetCounters = AssetCounters.OrderBy(x => x.TokenSymbol).ToList();
            IsCompletelyLoaded = true;
            StateHasChanged();
        }

        private void InvokeAssetLockPreviewerModal(AssetCounterModel model)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(AssetLockPreviewerModal.AssetCounterModel), model},
            };

            DialogService.Show<AssetLockPreviewerModal>($"{model.TokenInfo.Symbol}'s Locks", parameters, options);
        }

        private async Task OnTextToClipboardAsync(string text)
        {
            await ClipboardService.WriteTextAsync(text);
            AppDialogService.ShowSuccess("Contract Address copied to clipboard.");
        }
    }
}