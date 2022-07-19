﻿using Application.Common.Models;
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
        public bool IsConnected { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsSupported { get; set; }
        public string SupportMessage { get; set; }

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
            IsMyLocksLoaded = false;
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
                var balanceOutput = await LockTokenVaultService.GetLocksByInitiatorAsync();
                var s = balanceOutput.Locks[0].CreationTime;
                var ss = s.GetUniversalDateTime();
            }

            IsMyLocksLoaded = true;
            StateHasChanged();
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