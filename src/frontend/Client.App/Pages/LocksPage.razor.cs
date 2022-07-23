using Client.App.Models;
using Client.App.Pages.Assets.Modals;
using Client.App.Pages.Base;
using Client.App.Pages.Locks.Modals;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.Infrastructure.Exceptions;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages
{
    public partial class LocksPage : IPageBase, IDisposable
    {
        public bool IsMyLocksLoaded { get; set; }
        public bool IsMyClaimsLoaded { get; set; }
        public bool IsMyRefundsLoaded { get; set; }

        public bool IsConnected { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsSupported { get; set; }
        public string SupportMessage { get; set; }
        public List<LockModel> LockInitiatorTransactions { get; set; } = new();
        public List<LockForReceiverModel> LockReceiverTransactions { get; set; } = new();
        public List<LockRefundModel> LockRefunds { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Connected += HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
            IsConnected = await NightElfService.IsConnectedAsync();
        }

        private async void HandleNightElfExecutorConnected(object source, EventArgs e)
        {
            IsConnected = true;
            //if (!LockInitiatorTransactions.Any())
            //{
            //    await FetchInitiatorLocksAsync();
            //}

            //if (!LockReceiverTransactions.Any())
            //{
            //    await FetchReceiverLocksAsync();
            //}

            //if (!LockRefunds.Any())
            //{
            //    await FetchLockRefundsAsync();
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
            var isSupported = chains.Any(x => x.ChainIdBase58 == currentChain && x.IsLockingFeatureSupported);
            var supportedChains = chains.Where(x => x.IsLockingFeatureSupported).Select(x => x.ChainIdBase58).ToList();
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

                    await FetchInitiatorLocksAsync();
                    await FetchReceiverLocksAsync();
                    await FetchLockRefundsAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            IsProcessing = false;
            StateHasChanged();
        }

        private async Task FetchInitiatorLocksAsync()
        {
            IsMyLocksLoaded = false;
            LockInitiatorTransactions.Clear();
            StateHasChanged();

            var lockListOutput = await LockTokenVaultService.GetLocksByInitiatorAsync();
            lockListOutput.Locks ??= new();

            foreach (var @lock in lockListOutput.Locks)
            {
                LockInitiatorTransactions.Add(new LockModel(@lock));
            }

            LockInitiatorTransactions = LockInitiatorTransactions.Where(x => x.Status != "Revoked" && x.Status != "Claimed").ToList();
            LockInitiatorTransactions = LockInitiatorTransactions.OrderByDescending(x => x.Lock.StartTime.GetUniversalDateTime()).ToList();
            IsMyLocksLoaded = true;
            StateHasChanged();

            var tasks = new List<Task>();

            try
            {
                foreach (var @lock in LockInitiatorTransactions)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenService.GetTokenInfoAsync(new TokenGetTokenInfoInput()
                        {
                            Symbol = @lock.Lock.TokenSymbol
                        });
                        @lock.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch { }

            StateHasChanged();
        }

        private async Task FetchReceiverLocksAsync()
        {
            IsMyClaimsLoaded = false;
            LockReceiverTransactions.Clear();
            StateHasChanged();

            var lockListOutput = await LockTokenVaultService.GetLocksForReceiverAsync();
            lockListOutput.LockTransactions ??= new();

            foreach (var lockTransaction in lockListOutput.LockTransactions)
            {
                LockReceiverTransactions.Add(new LockForReceiverModel(lockTransaction.Lock, lockTransaction.Receiver));
            }

            LockReceiverTransactions = LockReceiverTransactions.Where(x => x.Status == "Locked" || x.Status == "Unlocked").ToList();
            LockReceiverTransactions = LockReceiverTransactions.OrderByDescending(x => x.Lock.StartTime.GetUniversalDateTime()).ToList();
            IsMyClaimsLoaded = true;
            StateHasChanged();

            var tasks = new List<Task>();

            try
            {
                foreach (var @lock in LockReceiverTransactions)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenService.GetTokenInfoAsync(new TokenGetTokenInfoInput()
                        {
                            Symbol = @lock.Lock.TokenSymbol
                        });
                        @lock.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch { }

            StateHasChanged();
        }


        private async Task FetchLockRefundsAsync()
        {
            IsMyRefundsLoaded = false;
            LockRefunds.Clear();
            StateHasChanged();

            var refundsOutput = await LockTokenVaultService.GetRefundsAsync();
            
            refundsOutput.Refunds ??= new();

            foreach (var refund in refundsOutput.Refunds)
            {
                LockRefunds.Add(new LockRefundModel(refund));
            }

            IsMyRefundsLoaded = true;
            StateHasChanged();

            var tasks = new List<Task>();

            try
            {
                foreach (var refundOutput in LockRefunds)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var tokenInfo = await TokenService.GetTokenInfoAsync(new TokenGetTokenInfoInput()
                        {
                            Symbol = refundOutput.Refund.TokenSymbol
                        });
                        refundOutput.SetTokenInfo(tokenInfo);
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch { }

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

        private async Task InvokeAddLockModalAsync()
        {
            var chain = await ChainService.FetchCurrentChainInfoAsync();

            var searchTokenDialog = DialogService.Show<SearchTokenModal>($"Search Token ({chain.ChainIdBase58} Chain)");
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
                    await FetchInitiatorLocksAsync();
                    await FetchReceiverLocksAsync();
                }
            }
        }

        private async Task InvokeClaimLockModalAsync(LockForReceiverModel lockModel)
        {
            var lockId = lockModel.Lock.LockId;

            var parameters = new DialogParameters();
            parameters.Add(nameof(ClaimLockModal.Model), new ClaimLockParameter()
            {
                LockId = lockId,
                ReceiverAddress = lockModel.Receiver.Receiver,
                AmountDisplay = lockModel.AmountDisplay
            });

            var dialog = DialogService.Show<ClaimLockModal>($"Claim from Lock #{lockId}", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchReceiverLocksAsync();
                await FetchInitiatorLocksAsync();
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
                await FetchInitiatorLocksAsync();
                await FetchLockRefundsAsync();
            }
        }

        private async Task InvokeClaimRefundModalAsync(LockRefundModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(ClaimLockRefundModal.Model), model);

            var dialog = DialogService.Show<ClaimLockRefundModal>($"Claim Refund for {model.TokenInfo.TokenName} ({model.TokenInfo.Symbol})", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchLockRefundsAsync();
            }
        }


        private void ClearData()
        {
            LockInitiatorTransactions.Clear();
            LockReceiverTransactions.Clear();
            LockRefunds.Clear();
            StateHasChanged();
        }

        public void Dispose()
        {
            NightElfExecutor.Connected -= HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}