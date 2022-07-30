using Application.Common.Dtos;
using Application.Common.Extensions;
using Application.Features.CrossChainOperations.Queries;
using Client.App.Pages.Assets.Modals;
using Client.App.Pages.Base;
using Client.App.Parameters;
using Client.App.SmartContractDto;
using Client.Infrastructure.Exceptions;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.App.Pages.Assets
{
    public partial class AssetsPage : IPageBase, IDisposable
    {
        public List<TokenBalanceInfoDto> TokenBalances { get; set; } = new();
        public bool IsAssetLoaded { get; set; }
        public bool IsConnected { get; set; }
        public bool FlagProcess { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsCrossChainOperationSectionEnabled { get; set; }
        public List<CrossChainPendingOperationDto> CrossChainOps { get; set; } = new();
        protected async override Task OnInitializedAsync()
        {
            NightElfExecutor.Connected += HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected += HandleNightElfExecutorDisconnected;
            IsConnected = await NightElfService.IsConnectedAsync();
        }

        private async void HandleNightElfExecutorConnected(object source, EventArgs e)
        {
            IsConnected = true;
            if (!TokenBalances.Any())
            {
                await FetchDataAsync();
            }
            StateHasChanged();
        }

        private void HandleNightElfExecutorDisconnected(object source, EventArgs e)
        {
            IsConnected = false;
            IsProcessing = false;
            FlagProcess = false;
            IsCrossChainOperationSectionEnabled = false;
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

        private async Task FetchDataAsync()
        {
            if (IsProcessing) return;

            TokenBalances = new List<TokenBalanceInfoDto>();
            IsAssetLoaded = false;
            FlagProcess = true;
            IsProcessing = true;
            StateHasChanged();

            var currentChainBase58 = await ChainService.FetchCurrentChainAsync();
            var walletAddress = await NightElfService.GetAddressAsync();

            try
            {
                if (string.IsNullOrEmpty(walletAddress)) throw new GeneralException("No Wallet found.");

                await FetchCrossChainOperationAsync();

                var result = await TokenManager.GetTokenBalancesAsync(currentChainBase58, walletAddress);
                if (result.Succeeded)
                {
                    TokenBalances = result.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            IsAssetLoaded = true;
            IsProcessing = false;
            StateHasChanged();

            FlagProcess = false;
            var tasks = new List<Task>();

            try
            {
                foreach (var tokenBalance in TokenBalances)
                {
                    if (FlagProcess)
                        break;

                    tasks.Add(InvokeAsync(async () =>
                    {
                        var balanceOutput = await TokenService.GetBalanceAsync(new TokenGetBalanceInput()
                        {
                            Symbol = tokenBalance.Token.Symbol,
                            Owner = walletAddress
                        });

                        tokenBalance.Balance = balanceOutput.Balance.ToAmountDisplay(tokenBalance.Token.Decimals);
                    }));
                }
            }
            catch
            {

            }

            await Task.WhenAll(tasks);
            StateHasChanged();
        }

        private async Task FetchCrossChainOperationAsync()
        {
            var currentChain = await ChainService.FetchCurrentChainInfoAsync();

            if (currentChain.ChainType != "Side")
            {
                IsCrossChainOperationSectionEnabled = false;
                CrossChainOps.Clear();
                StateHasChanged();
                return;
            }

            var walletAddress = await NightElfService.GetAddressAsync();

            if (string.IsNullOrEmpty(walletAddress)) return;

            try
            {
                var result = await _exceptionHandler.HandlerRequestTaskAsync(() => CrossChainOperationManager.GetPendingOperationAsync(new GetPendingOperationsQuery()
                {
                    From = walletAddress,
                    ContractName = "Token",
                    IssueChainId = currentChain.ChainId
                }));

                if (result.Succeeded)
                {
                    CrossChainOps = result.Data;
                    IsCrossChainOperationSectionEnabled = true;
                    StateHasChanged();
                }
            }
            catch { }
        }

        private async Task<(bool, List<string>)> ValidateSupportedChainAsync()
        {
            var chains = await ChainService.FetchSupportedChainsAsync();
            var currentChain = await ChainService.FetchCurrentChainAsync();
            var isSupported = chains.Any(x => x.ChainIdBase58 == currentChain && x.IsTokenCreationFeatureSupported);
            var supportedChains = chains.Where(x => x.IsTokenCreationFeatureSupported).Select(x => x.ChainIdBase58).ToList();
            return (isSupported, supportedChains);
        }

        private async Task OnCreateNewTokenAsync()
        {
            var result = await ValidateSupportedChainAsync();

            if (!result.Item1)
            {
                var message = "Token Creation feature is not supported in thischain.";

                if (result.Item2.Any())
                {
                    message = $"Token Creation feature is not supported in thischain. Currently, it is only supported on the following chains: <br><ul>{string.Join("", result.Item2.Select(x => $"<li>• {x}</li>").ToList())}</ul>";
                }

                AppDialogService.ShowError(message);
                return;
            }

            var isConnected = await NightElfService.IsConnectedAsync();
            if (!isConnected)
            {
                AppDialogService.ShowError("Connect your wallet first.");
                return;
            }

            var dialog = DialogService.Show<CreateTokenModal>($"Create New Token");
            await dialog.Result;
        }

        private async Task OnIssueTokenAsync(TokenBalanceInfoDto balanceInfo)
        {
            var isConnected = await NightElfService.IsConnectedAsync();
            if (!isConnected)
            {
                AppDialogService.ShowError("Connect your wallet first.");
                return;
            }

            var parameters = new DialogParameters()
            {
                { nameof(IssueTokenModal.Model), new IssueTokenParameter() { Symbol = balanceInfo.Token.Symbol, TokenName = balanceInfo.Token.Name, Decimals = balanceInfo.Token.Decimals, IssuedChainId = balanceInfo.Token.IssueChainId.ToChainId() } }
            };

            var dialog = DialogService.Show<IssueTokenModal>($"Issue Token", parameters);
            await dialog.Result;
        }

        private async Task OnConfirmTransactionAsync(CrossChainPendingOperationDto op)
        {
            var isConnected = await NightElfService.IsConnectedAsync();
            if (!isConnected)
            {
                AppDialogService.ShowError("Connect your wallet first.");
                return;
            }

            var parameters = new DialogParameters()
            {
                { nameof(ConfirmTokenCrossChainTransactionModal.Model), op }
            };

            var dialog = DialogService.Show<ConfirmTokenCrossChainTransactionModal>($"Confirm Cross Chain Transaction", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private void ClearData()
        {
            TokenBalances = new();
            CrossChainOps = new();
            StateHasChanged();
        }

        public void Dispose()
        {
            IsProcessing = false;
            FlagProcess = true;
            NightElfExecutor.Connected -= HandleNightElfExecutorConnected;
            NightElfExecutor.Disconnected -= HandleNightElfExecutorDisconnected;
        }
    }
}