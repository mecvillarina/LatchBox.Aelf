using Client.App.Models;
using Client.App.Pages.Base;
using Client.App.Pages.Launchpads.Modals;
using Client.App.SmartContractDto;
using Client.Infrastructure.Exceptions;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Launchpads
{
    public partial class ManageLaunchpadsPage : IPageBase, IDisposable
    {
        public bool IsMyLaunchpadLoaded { get; set; }
        public bool IsInvestedLaunchpadLoaded { get; set; }

        public bool IsConnected { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsSupported { get; set; }
        public string SupportMessage { get; set; }
        public TokenInfo NativeTokenInfo { get; set; }
        public List<MyLaunchpadModel> MyLaunchpadList { get; set; } = new();
        public List<InvestedLaunchpadModel> InvestedLaunchpadList { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Connected += HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
            IsConnected = await NightElfService.IsConnectedAsync();
        }

        private async void HandleNightElfExecutorConnected(object source, EventArgs e)
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
                    var walletAddress = await NightElfService.GetAddressAsync();
                    if (string.IsNullOrEmpty(walletAddress)) throw new GeneralException("No Wallet found.");

                    NativeTokenInfo = await TokenService.GetNativeTokenInfoAsync();
                    await FetchMyLaunchpadsAsync();
                    await FetchInvestedLaunchpadsAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            IsProcessing = false;
            StateHasChanged();
        }

        private async Task FetchMyLaunchpadsAsync()
        {
            IsMyLaunchpadLoaded = false;
            MyLaunchpadList.Clear();
            StateHasChanged();

            var output = await LaunchpadService.GetCrowdSalesByInitiatorAsync();
            output ??= new();

            MyLaunchpadList = output.List.Select(x => new MyLaunchpadModel(x)).ToList();
            IsMyLaunchpadLoaded = true;
            StateHasChanged();
        }

        private async Task FetchInvestedLaunchpadsAsync()
        {
            IsInvestedLaunchpadLoaded = false;
            InvestedLaunchpadList.Clear();
            StateHasChanged();

            var output = await LaunchpadService.GetCrowdSalesByInvestorAsync();
            output ??= new();

            InvestedLaunchpadList = output.List.Select(x => new InvestedLaunchpadModel(x)).ToList();
            IsInvestedLaunchpadLoaded = true;
            StateHasChanged();
        }

        private void OnViewMyLaunchpad(MyLaunchpadModel model)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.CrowdSaleId), model.Launchpad.Id},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"{model.Launchpad.Name}", parameters, options);
        }

        private void OnViewInvestedLaunchpad(InvestedLaunchpadModel model)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters()
            {
                 { nameof(LaunchpadPreviewerModal.CrowdSaleId), model.Launchpad.Id},
                 { nameof(LaunchpadPreviewerModal.NativeTokenInfo), NativeTokenInfo},
            };

            DialogService.Show<LaunchpadPreviewerModal>($"{model.Launchpad.Name}", parameters, options);
        }

        private void InvokeLockPreviewerModal(long lockId)
        {
            NavigationManager.NavigateTo($"/locks/{lockId}");
        }

        private void ClearData()
        {
            MyLaunchpadList.Clear();
            InvestedLaunchpadList.Clear();
            StateHasChanged();
        }

        public void Dispose()
        {
            NightElfExecutor.Connected -= HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}