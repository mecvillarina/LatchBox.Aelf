using AElf;
using AElf.Client.Dto;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using Client.Pages.Tokens.Modals;
using Client.Parameters;
using MudBlazor;

namespace Client.Pages.Tokens
{
    public partial class AssetsPage
    {
        public bool IsLoaded { get; set; }
        public bool IsCompletelyLoaded { get; set; }
        public string WalletAddress { get; set; }
        public List<TokenInfoWithBalance> TokenInfoWithBalanceList { get; set; } = new();
        public int MainChainId { get; set; }
        public int SideChainId { get; set; }
        public string MainChain { get; set; }
        public string SideChain { get; set; }
        public ChainStatusDto MainChainStatus { get; set; }
        public ChainStatusDto SideChainStatus { get; set; }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    WalletAddress = await WalletManager.GetWalletAddressAsync();
                    MainChainId = BlockchainManager.GetMainChainId();
                    SideChainId = BlockchainManager.GetSideChainId();
                    MainChain = $"Main {MainChainId.ToChainName()}";
                    SideChain = $"Side {SideChainId.ToChainName()}";
                    await FetchDataAsync();
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            IsCompletelyLoaded = false;
            StateHasChanged();

            TokenInfoWithBalanceList.Clear();

            MainChainStatus = BlockchainManager.FetchMainChainStatus();
            SideChainStatus = BlockchainManager.FetchSideChainStatus();

            var nativeTokenInfo = await TokenManager.GetNativeTokenInfoOnMainChainAsync();
            var nativeToken = new TokenInfoBase(nativeTokenInfo);
            TokenInfoWithBalanceList.Add(new TokenInfoWithBalance(nativeToken)
            {
                IsNative = true
            });

            var tokenSymbolList = await TokenManager.GetTokenSymbolsFromStorageAsync();

            foreach (var symbol in tokenSymbolList)
            {
                var sideChainToken = await TokenManager.GetCacheTokenInfoAsync(SideChainId, symbol);
                if (sideChainToken == null)
                {
                    try
                    {
                        var sideChainTokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(symbol);
                        sideChainToken = new TokenInfoBase(sideChainTokenInfo);
                    }
                    catch
                    {
                        sideChainToken = new();
                    }
                }

                if (!string.IsNullOrEmpty(sideChainToken.Symbol))
                {
                    await TokenManager.CacheTokenInfoAsync(SideChainId, sideChainToken);
                    TokenInfoWithBalance tokenInfo = new TokenInfoWithBalance(sideChainToken);
                    TokenInfoWithBalanceList.Add(tokenInfo);
                }
                else
                {
                    await TokenManager.RemoveFromTokenSymbolsStorageAsync(symbol);
                }
            }

            TokenInfoWithBalanceList = TokenInfoWithBalanceList.OrderByDescending(x => x.IsNative).ThenBy(x => x.TokenName).ToList();
            IsLoaded = true;
            StateHasChanged();

            await FetchBalanceAsync();

            IsCompletelyLoaded = true;
            StateHasChanged();
        }


        private async Task FetchBalanceAsync()
        {
            IsCompletelyLoaded = false;

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                tokenInfo.MainChainBalance = null;
                tokenInfo.SideChainBalance = null;
            }

            StateHasChanged();

            MainChainStatus = BlockchainManager.FetchMainChainStatus();
            SideChainStatus = BlockchainManager.FetchSideChainStatus();

            try
            {
                var tasks = new List<Task>();
                foreach (var tokenInfo in TokenInfoWithBalanceList)
                {
                    tasks.Add(InvokeAsync(async () =>
                    {
                        var mainChainGetBalanceOutput = await TokenManager.GetBalanceOnMainChainAsync(tokenInfo.Symbol);
                        tokenInfo.MainChainBalance = mainChainGetBalanceOutput.Balance;
                        StateHasChanged();
                    }));

                    tasks.Add(InvokeAsync(async () =>
                    {
                        var sideChainGetBalanceOutput = await TokenManager.GetBalanceOnSideChainAsync(tokenInfo.Symbol);
                        tokenInfo.SideChainBalance = sideChainGetBalanceOutput.Balance;
                        StateHasChanged();
                    }));
                    //var mainChainGetBalanceOutput = await TokenManager.GetBalanceOnMainChainAsync(tokenInfo.Symbol);
                    //tokenInfo.MainChainBalance = mainChainGetBalanceOutput.Balance;
                    //var sideChainGetBalanceOutput = await TokenManager.GetBalanceOnSideChainAsync(tokenInfo.Symbol);
                    //tokenInfo.SideChainBalance = sideChainGetBalanceOutput.Balance;
                    //StateHasChanged();
                }

                await Task.WhenAll(tasks);
            }
            catch
            {

            }
           

            IsCompletelyLoaded = true;
            StateHasChanged();
        }

        private async Task InvokeAddTokenModalAsync()
        {
            var dialog = DialogService.Show<AddExistingTokenModal>($"Add Existing Token");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private async Task InvokeCreateTokenModalAsync()
        {
            var dialog = DialogService.Show<CreateTokenModal>($"Create New Token");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }

        private async Task InvokeRemoveFromList(TokenInfoWithBalance tokenInfo)
        {
            TokenInfoWithBalanceList.Remove(tokenInfo);
            StateHasChanged();
            await TokenManager.RemoveFromTokenSymbolsStorageAsync(tokenInfo.Symbol);
        }

        private async Task InvokeIssueTokenModalAsync(TokenInfoWithBalance tokenInfo)
        {
            var parameters = new DialogParameters()
            {
                { nameof(IssueTokenModal.Model), new IssueTokenParameter() { Symbol = tokenInfo.Symbol, TokenName = tokenInfo.TokenName, Decimals = tokenInfo.Decimals, IssuedChainId = tokenInfo.IssueChainId } }
            };

            var dialog = DialogService.Show<IssueTokenModal>($"Issue Token ({ChainHelper.ConvertChainIdToBase58(tokenInfo.IssueChainId)} Chain)", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchBalanceAsync();
            }
        }
    }
}