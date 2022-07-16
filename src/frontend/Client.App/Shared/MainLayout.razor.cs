﻿using Client.App.Models;
using Client.App.Pages.Base;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Client.App.Shared
{
    public partial class MainLayout : IPageBase, IAsyncDisposable
    {
        private bool DrawerOpen { get; set; } = true;
        private MudTheme CurrentTheme { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsNavMenuPanelExpanded { get; set; }

        public string CurrentChainIdBase58 { get; set; }
        public string CurrentChainExplorerUrl { get; set; }
        public string CurrentChainNodeUrl => $"{CurrentChainExplorerUrl}/chain";

        public string CurrentChainDisplay => $"Chain ({CurrentChainIdBase58})";

        public List<SupportedChainModel> SupportedChains { get; set; } = new();

        public NightElfModel NightElf { get; set; }

        protected override void OnInitialized()
        {
            NightElf = new NightElfModel();
            CurrentTheme = _clientPreferenceManager.GetCurrentTheme();
            FetchDataExecutor.StartExecuting();
            NightElfExecutor.StartExecuting();
        }

        protected async override Task OnInitializedAsync()
        {
            await AppBreakpointService.InitAsync();
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {

            try
            {
                if (firstRender)
                {
                    await FetchChainDataAsync();
                }
            }
            catch { }
        }

        private async Task FetchChainDataAsync()
        {
            IsLoaded = await ChainService.FetchChainDataAsync();

            if (IsLoaded)
            {
                var supportedChains = await ChainService.FetchSupportedChainsAsync();
                SupportedChains = supportedChains.Select(x => new SupportedChainModel() { ExplorerUrl = x.Explorer, ChainType = x.ChainType, ChainIdBase58 = x.ChainIdBase58 }).ToList();
                CurrentChainIdBase58 = await ChainService.FetchCurrentChainAsync();
                CurrentChainExplorerUrl = SupportedChains.First(x => x.ChainIdBase58 == CurrentChainIdBase58).ExplorerUrl;
                await InitNightElfAsync();
            }

            StateHasChanged();
        }

        private async Task InitNightElfAsync()
        {
            NightElf.HasExtension = await NightElfService.HasNightElfAsync();

            if (NightElf.HasExtension)
            {
                await NightElfService.InitializeNightElfAsync("aelf.LatchBox", CurrentChainNodeUrl);
                NightElf.IsConnected = await NightElfService.IsConnectedAsync();

                if (NightElf.IsConnected)
                {
                    NightElf.WalletAddress = await NightElfService.GetAddressAsync();
                }
            }
        }

        private async Task OnSetChainAsync(string chainIdBase58)
        {
            Console.WriteLine(chainIdBase58);

            var currentChainIdBase58 = await ChainService.FetchCurrentChainAsync();
            if (chainIdBase58 == currentChainIdBase58) return;

            var data = await ChainService.FetchSupportedChainsAsync();
            if (data.Any(x => x.ChainIdBase58 == chainIdBase58))
            {
                await ChainService.SetCurrentChainAsync(chainIdBase58);
                NavigationManager.NavigateTo(NavigationManager.Uri, true);
            }
        }

        private async Task OnConnectWalletAsync()
        {
            NightElf.HasExtension = await NightElfService.HasNightElfAsync();

            if (!NightElf.HasExtension)
            {
                AppDialogService.Show("Please <a href=\"https://chrome.google.com/webstore/detail/aelf-explorer-extension/mlmlhipeonlflbcclinpbmcjdnpnmkpf\" target=\"_blank\"><u>download and install</u></a> NightELF browser extension.");
            }
            else
            {
                await NightElfService.InitializeNightElfAsync("aelf.LatchBox", CurrentChainNodeUrl);
                var loginResult = await NightElfService.LoginAsync();
                if (loginResult)
                {
                    NightElf.WalletAddress = await NightElfService.GetAddressAsync();
                    NightElf.IsConnected = await NightElfService.IsConnectedAsync();
                    StateHasChanged();
                }
            }
        }

        private void HandleNightElfExecutorDisconnected(object source, EventArgs e)
        {
            if (string.IsNullOrEmpty(NightElf.WalletAddress)) return;
            AppDialogService.ShowError("Wallet has been disconnected");
            NightElf.IsConnected = false;
        }

        private async Task OnDisconnectWalletAsync()
        {
            await NightElfService.LogoutAsync();
            NightElf.Clear();
            NightElfExecutor.InvokeDisconnect();
            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            await AppBreakpointService.DisposeAsync();
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}