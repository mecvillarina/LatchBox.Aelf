using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Client.Models;
using Client.Pages.Modals;
using Client.Pages.Vestings.Modals;
using Client.Parameters;
using MudBlazor;
using System.Numerics;

namespace Client.Pages.Vestings
{
    public partial class MyVestingsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }
        public WalletInformation Wallet { get; set; }

        public List<VestingModel> Vestings { get; set; } = new();

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        Wallet = await WalletManager.GetWalletInformationAsync();
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

            Vestings.Clear();

            var result = await VestingTokenVaultManager.InitializeAsync();
            var vestingListOutput = await VestingTokenVaultManager.GetVestingsByInitiatorAsync(Wallet.Address);

            foreach (var vesting in vestingListOutput.Transactions)
            {
                Vestings.Add(new VestingModel(vesting));
            }

            Vestings = Vestings.OrderByDescending(x => x.Vesting.CreationTime).ToList();

            IsLoaded = true;
            StateHasChanged();

            foreach (var vesting in Vestings)
            {
                var tokenInfo = await TokenManager.GetTokenInfoAsync(vesting.Vesting.TokenSymbol);
                vesting.SetTokenInfo(tokenInfo);
            }

            IsCompletelyLoaded = true;
            StateHasChanged();
        }

        private async Task InvokeAddVestingModalAsync()
        {
            var searchTokenDialog = DialogService.Show<SearchTokenModal>($"Search Token");
            var searchTokenDialogResult = await searchTokenDialog.Result;

            if (!searchTokenDialogResult.Cancelled)
            {
                var tokenInfo = (TokenInfo)searchTokenDialogResult.Data;

                var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters();
                parameters.Add(nameof(AddVestingModal.TokenInfo), tokenInfo);

                var dialog = DialogService.Show<AddVestingModal>($"Add New Vesting", parameters, options);
                var dialogResult = await dialog.Result;

                if (!dialogResult.Cancelled)
                {
                    await FetchDataAsync();
                }
            }
        }

        private async Task InvokeRevokeVestingModalAsync(VestingModel vestingModel)
        {
            //var vestingIndex = vestingModel.Transaction.VestingIndex;

            //var parameters = new DialogParameters();
            //parameters.Add(nameof(RevokeVestingModal.VestingTransaction), vestingModel.Transaction);
            //parameters.Add(nameof(RevokeVestingModal.Model), new RevokeVestingParameter() { VestingIndex = vestingIndex });

            //var dialog = DialogService.Show<RevokeVestingModal>($"Revoke Vesting #{vestingIndex}", parameters);
            //var dialogResult = await dialog.Result;

            //if (!dialogResult.Cancelled)
            //{
            //    await FetchDataAsync();
            //}
        }

        private void InvokeVestingPreviewerModal(long vestingId)
        {
            //var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            //var parameters = new DialogParameters()
            //{
            //     { nameof(VestingPreviewerModal.VestingIndex), vestingIndex},
            //};

            //DialogService.Show<VestingPreviewerModal>($"Vesting #{vestingIndex}", parameters, options);
        }

        private async Task OnTextToClipboardAsync(string text)
        {
            await ClipboardService.WriteTextAsync(text);
            AppDialogService.ShowSuccess("Contract Address copied to clipboard.");
        }
    }
}