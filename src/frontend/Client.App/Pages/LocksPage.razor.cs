using Application.Common.Models;
using Client.App.Models;
using Client.App.Pages.Base;
using Client.App.SmartContractDto;
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
        public bool IsConnected { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsSupported { get; set; }
        public string SupportMessage { get; set; }
        public List<LockModel> LockInitiatorTransactions { get; set; } = new();
        public List<LockForReceiverModel> LockReceiverTransactions { get; set; } = new();

        private MudTabs tabs { get; set; }
        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Connected += HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
            IsConnected = await NightElfService.IsConnectedAsync();
        }

        private async void HandleNightElfExecutorConnected(object source, EventArgs e)
        {
            IsConnected = true;
            //if (!TokenBalances.Any())
            //{
            //    await FetchDataAsync();
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
                await FetchInitiatorLocksAsync();
                await FetchReceiverLocksAsync();

                //load refunds
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
            foreach (var @lock in lockListOutput.Locks)
            {
                LockInitiatorTransactions.Add(new LockModel(@lock));
            }

            LockInitiatorTransactions = LockInitiatorTransactions.Where(x => x.Status != "Revoked").ToList();
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

        private void InvokeLockPreviewerModal(long lockId)
        {
            //var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            //var parameters = new DialogParameters()
            //{
            //     { nameof(LockPreviewerModal.LockId), lockId},
            //};

            //DialogService.Show<LockPreviewerModal>($"Lock #{lockId}", parameters, options);
        }

        private void ClearData()
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            NightElfExecutor.Connected -= HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}