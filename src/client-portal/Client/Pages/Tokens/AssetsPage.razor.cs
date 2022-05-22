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
                    MainChainId = await BlockchainManager.GetMainChainIdAsync();
                    SideChainId = await BlockchainManager.GetSideChainIdAsync();
                    MainChain = $"Main {MainChainId.ToStringChainId()}";
                    SideChain = $"Side {SideChainId.ToStringChainId()}";
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

            MainChainStatus = await BlockchainManager.GetMainChainStatusAsync();
            SideChainStatus = await BlockchainManager.GetSideChainStatusAsync();

            var sideChainNativeTokenInfo = await TokenManager.GetNativeTokenInfoOnSideChainAsync();
            var sideChainNativeToken = new TokenInfoBase(sideChainNativeTokenInfo);
            TokenInfoWithBalanceList.Add(new TokenInfoWithBalance(sideChainNativeToken)
            {
                IsNative = true
            });

            var tokenSymbolList = await TokenManager.GetTokenSymbolsFromStorageAsync();

            foreach (var symbol in tokenSymbolList)
            {
                var sideChainToken = await TokenManager.GetCacheTokenInfoAsync(SideChainId, symbol);
                if (sideChainToken == null)
                {
                    var sideChainTokenInfo = await TokenManager.GetTokenInfoOnSideChainAsync(symbol);
                    sideChainToken = new TokenInfoBase(sideChainTokenInfo);
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

            //foreach (var tokenInfo in TokenInfoWithBalanceList)
            //{
            //    var mainChainToken = await TokenManager.GetTokenInfoOnMainChainAsync(tokenInfo.Symbol);
            //    tokenInfo.MainChainSupply = mainChainToken.Supply;
            //    StateHasChanged();
            //}

            //foreach (var tokenInfo in TokenInfoWithBalanceList)
            //{
            //    if (!tokenInfo.SideChainSupply.HasValue)
            //    {
            //        var sideChainToken = await TokenManager.GetTokenInfoOnSideChainAsync(tokenInfo.Symbol);
            //        tokenInfo.SideChainSupply = sideChainToken.Supply;
            //        StateHasChanged();
            //    }
            //}

            await FetchBalanceAsync();

            IsCompletelyLoaded = true;
            StateHasChanged();
        }


        private async Task FetchBalanceAsync()
        {
            IsCompletelyLoaded = false;
            StateHasChanged();

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                tokenInfo.MainChainBalance = null;
                tokenInfo.SideChainBalance = null;
            }

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                var mainChainGetBalanceOutput = await TokenManager.GetBalanceOnMainChainAsync(MainChainStatus, tokenInfo.Symbol);
                tokenInfo.MainChainBalance = mainChainGetBalanceOutput.Balance;
                var sideChainGetBalanceOutput = await TokenManager.GetBalanceOnSideChainAsync(SideChainStatus, tokenInfo.Symbol);
                tokenInfo.SideChainBalance = sideChainGetBalanceOutput.Balance;
                StateHasChanged();
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