using Client.App.Models;
using Client.App.Pages.Assets.Modals;
using Client.App.Pages.Base;
using Client.App.Pages.Launchpads.Modals;
using Client.App.Pages.Vestings.Modals;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.Launchpad;
using Client.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads
{
    public partial class AllLaunchpadsPage : IPageBase, IDisposable
    {
        [Parameter]
        public long? CrowdSaleId { get; set; }

        public bool IsLoaded { get; set; }

        public bool IsConnected { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsSupported { get; set; }
        public string SupportMessage { get; set; }
        public int LaunchpadStatus { get; set; }
        public TokenInfo NativeTokenInfo { get; set; }
        public List<LaunchpadModel> LaunchpadList { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Connected += HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
            IsConnected = await NightElfService.IsConnectedAsync();
        }

        private void HandleNightElfExecutorConnected(object source, EventArgs e)
        {
            IsConnected = true;
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
            var isSupported = chains.Any(x => x.ChainIdBase58 == currentChain && x.IsLaunchpadFeatureSupported);
            var supportedChains = chains.Where(x => x.IsLaunchpadFeatureSupported).Select(x => x.ChainIdBase58).ToList();
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
                    var isConnected = await NightElfService.IsConnectedAsync();

                    if (isConnected && CrowdSaleId.HasValue && CrowdSaleId.Value > 0)
                    {
                        NativeTokenInfo = await TokenService.GetNativeTokenInfoAsync();
                        InvokeLaunchpadPreviewerModal(CrowdSaleId.Value);
                    }

                    var walletAddress = await NightElfService.GetAddressAsync();
                    if (string.IsNullOrEmpty(walletAddress)) throw new GeneralException("No Wallet found.");
                    
                    NativeTokenInfo = await TokenService.GetNativeTokenInfoAsync();
                    await FetchLaunchpadsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            IsProcessing = false;
            StateHasChanged();
        }

        private async Task FetchLaunchpadsAsync()
        {
            IsLoaded = false;
            StateHasChanged();

            bool isUpcoming = false;
            bool isOngoing = false;

            switch (LaunchpadStatus)
            {
                case 1: isUpcoming = true; break;
                case 2: isOngoing = true; break;
                default: isUpcoming = isOngoing = false; break;
            }

            var output = await LaunchpadService.GetCrowdSalesAsync(new CrowdSaleGetCrowdSalesInput()
            {
                IsOngoing = isOngoing,
                IsUpcoming = isUpcoming,
            });

            output ??= new();
            LaunchpadList = output.List.Select(x => new LaunchpadModel(x)).ToList();

            IsLoaded = true;
            StateHasChanged();
        }

        private void InvokeLaunchpadPreviewerModal(long crowdSaleId)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.CrowdSaleId), crowdSaleId},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"Launchpad #{crowdSaleId}", parameters, options);
        }

        public async Task OnStatusChanged(int value)
        {
            LaunchpadStatus = value;
            await FetchDataAsync();
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