using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Models;
using Client.Pages.Vestings.Modals;
using Client.Parameters;
using MudBlazor;
using System.Numerics;

namespace Client.Pages.Vestings
{
    public partial class MyVestingClaimsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }
        public string WalletAddress { get; set; }
        public string ContractLink => $"{BlockchainManager.SideChainExplorer}/address/{VestingTokenVaultManager.ContactAddress}";
        public List<VestingReceiverModel> Vestings { get; set; } = new();

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

            var vestingListOutput = await VestingTokenVaultManager.GetVestingsForReceiverAsync(WalletAddress);

            foreach (var transactionOutput in vestingListOutput.Transactions)
            {
                Vestings.Add(new VestingReceiverModel(transactionOutput));
            }

            Vestings = Vestings.OrderBy(x => x.Vesting.IsRevoked).ThenBy(x => x.Period.UnlockTime).Select(x => new
            {
                IsPastTime = x.Period.UnlockTime.ToDateTime() > DateTime.UtcNow,
                Vesting = x
            }).OrderBy(x => x.IsPastTime).Select(x => x.Vesting).ToList();

            IsLoaded = true;
            StateHasChanged();

            try
            {
                var tasks = new List<Task>();

                foreach (var vesting in Vestings)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(vesting.Vesting.TokenSymbol);
                        vesting.SetTokenInfo(tokenInfo);
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

        private async Task InvokeClaimVestingModalAsync(VestingReceiverModel vestingModel)
        {
            var vestingId = vestingModel.Vesting.VestingId;
            var periodName = vestingModel.Period.Name;

            var parameters = new DialogParameters();
            parameters.Add(nameof(ClaimVestingModal.Model), new ClaimVestingParameter()
            {
                VestingId = vestingModel.Vesting.VestingId,
                PeriodId = vestingModel.Period.PeriodId,
                PeriodName = periodName,
                ReceiverAddress = vestingModel.Receiver.Address.ToStringAddress(),
                AmountDisplay = vestingModel.AmountDisplay
            });

            var dialog = DialogService.Show<ClaimVestingModal>($"Claim from Vesting #{vestingId} - {periodName}", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private void InvokeVestingPreviewerModal(long vestingId)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(VestingPreviewerModal.VestingId), vestingId},
            };

            DialogService.Show<VestingPreviewerModal>($"Vesting #{vestingId}", parameters, options);
        }
    }
}