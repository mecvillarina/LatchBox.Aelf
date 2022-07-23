using Client.App.Models;
using Client.App.Pages.Assets.Modals;
using Client.App.Pages.Base;
using Client.App.Pages.Vestings.Modals;
using Client.App.SmartContractDto;
using Client.Infrastructure.Exceptions;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages
{
    public partial class VestingsPage : IPageBase, IDisposable
    {
        public bool IsMyVestingsLoaded { get; set; }
        public bool IsMyClaimsLoaded { get; set; }
        public bool IsMyRefundsLoaded { get; set; }

        public bool IsConnected { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsSupported { get; set; }
        public string SupportMessage { get; set; }
        public List<VestingByInitiatorModel> VestingInitiatorTransactions { get; set; } = new();
        public List<VestingReceiverModel> VestingReceiverTransactions { get; set; } = new();
        public List<VestingRefundModel> VestingRefunds { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Connected += HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
            IsConnected = await NightElfService.IsConnectedAsync();
        }

        private async void HandleNightElfExecutorConnected(object source, EventArgs e)
        {
            IsConnected = true;
            //if (!VestingInitiatorTransactions.Any())
            //{
            //    await FetchInitiatorVestingsAsync();
            //}

            //if (!VestingReceiverTransactions.Any())
            //{
            //    await FetchReceiverVestingsAsync();
            //}

            //if (!VestingRefunds.Any())
            //{
            //    await FetchVestingRefundsAsync();
            //}

            StateHasChanged();
        }

        private void HandleNightElfExecutorDisconnected(object source, EventArgs e)
        {
            IsConnected = false;
            IsProcessing = false;
            ClearData();
            StateHasChanged();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
            }
        }

        private async Task<(bool, List<string>)> ValidateSupportedChainAsync()
        {
            var chains = await ChainService.FetchSupportedChainsAsync();
            var currentChain = await ChainService.FetchCurrentChainAsync();
            var isSupported = chains.Any(x => x.ChainIdBase58 == currentChain && x.IsVestingFeatureSupported);
            var supportedChains = chains.Where(x => x.IsVestingFeatureSupported).Select(x => x.ChainIdBase58).ToList();
            return (isSupported, supportedChains);
        }

        private async Task FetchDataAsync()
        {
            if (IsProcessing) return;

            IsProcessing = true;
            SupportMessage = String.Empty;

            var result = await ValidateSupportedChainAsync();

            IsSupported = result.Item1;

            if (!IsSupported)
            {
                if (result.Item2.Any())
                {
                    var message = $"Currently, it is only supported on the following chains: <br><ul>{string.Join("", result.Item2.Select(x => $"<li>• {x}</li>").ToList())}</ul>";
                    SupportMessage = message;
                }
            }
            else
            {
                try
                {
                    var walletAddress = await NightElfService.GetAddressAsync();
                    if (string.IsNullOrEmpty(walletAddress)) throw new GeneralException("No Wallet found.");

                    await FetchInitiatorVestingsAsync();
                    await FetchReceiverVestingsAsync();
                    await FetchVestingRefundsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            IsProcessing = false;
            StateHasChanged();
        }

        private async Task FetchInitiatorVestingsAsync()
        {
            IsMyVestingsLoaded = false;
            VestingInitiatorTransactions.Clear();
            StateHasChanged();

            var vestingListOutput = await VestingTokenVaultService.GetVestingsByInitiatorAsync();
            vestingListOutput.Transactions ??= new();

            foreach (var transaction in vestingListOutput.Transactions)
            {
                VestingInitiatorTransactions.Add(new VestingByInitiatorModel(transaction));
            }

            VestingInitiatorTransactions = VestingInitiatorTransactions.Where(x => x.StatusDisplay != "Revoked" && x.StatusDisplay != "Completed").ToList();
            VestingInitiatorTransactions = VestingInitiatorTransactions.OrderByDescending(x => x.Vesting.CreationTime.GetUniversalDateTime()).ToList();
            IsMyVestingsLoaded = true;
            StateHasChanged();

            var tasks = new List<Task>();

            try
            {
                foreach (var transaction in VestingInitiatorTransactions)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenService.GetTokenInfoAsync(new TokenGetTokenInfoInput()
                        {
                            Symbol = transaction.Vesting.TokenSymbol
                        });
                        transaction.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch { }

            StateHasChanged();
        }

        private async Task FetchReceiverVestingsAsync()
        {
            IsMyClaimsLoaded = false;
            VestingReceiverTransactions.Clear();
            StateHasChanged();

            var vestingListOutput = await VestingTokenVaultService.GetVestingsForReceiverAsync();
            vestingListOutput.Transactions ??= new();

            foreach (var transactionOutput in vestingListOutput.Transactions)
            {
                VestingReceiverTransactions.Add(new VestingReceiverModel(transactionOutput));
            }

            VestingReceiverTransactions = VestingReceiverTransactions.Where(x => x.StatusDisplay == "Locked" || x.StatusDisplay == "Unlocked").ToList();
            VestingReceiverTransactions = VestingReceiverTransactions.OrderBy(x => x.Vesting.IsRevoked).ThenBy(x => x.Period.UnlockTime.GetUniversalDateTime())
                .Select(x => new
                {
                    IsPastTime = x.Period.UnlockTime.GetUniversalDateTime() > DateTime.UtcNow,
                    Vesting = x
                }).OrderBy(x => x.IsPastTime).Select(x => x.Vesting).ToList();

            IsMyClaimsLoaded = true;
            StateHasChanged();

            var tasks = new List<Task>();

            try
            {
                foreach (var vesting in VestingReceiverTransactions)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenService.GetTokenInfoAsync(new TokenGetTokenInfoInput()
                        {
                            Symbol = vesting.Vesting.TokenSymbol
                        });
                        vesting.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch { }

            StateHasChanged();
        }


        private async Task FetchVestingRefundsAsync()
        {
            IsMyRefundsLoaded = false;
            VestingRefunds.Clear();
            StateHasChanged();

            var refundsOutput = await VestingTokenVaultService.GetRefundsAsync();
            refundsOutput.Refunds ??= new();
            foreach (var refund in refundsOutput.Refunds)
            {
                VestingRefunds.Add(new VestingRefundModel(refund));
            }


            IsMyRefundsLoaded = true;
            StateHasChanged();

            var tasks = new List<Task>();

            try
            {
                foreach (var refund in VestingRefunds)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenService.GetTokenInfoAsync(new TokenGetTokenInfoInput()
                        {
                            Symbol = refund.Refund.TokenSymbol
                        });
                        refund.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch { }

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

        private void ClearData()
        {
            VestingInitiatorTransactions.Clear();
            VestingReceiverTransactions.Clear();
            VestingRefunds.Clear();
            StateHasChanged();
        }

        public void Dispose()
        {
            NightElfExecutor.Connected -= HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}