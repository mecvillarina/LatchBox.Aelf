using AElf;
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
        public string MainChain { get; set; }
        public string SideChain { get; set; }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        WalletAddress = await WalletManager.GetWalletAddressAsync();
                        MainChain = $"Main {(await BlockchainManager.GetMainChainIdAsync()).ToStringChainId()}";
                        SideChain = $"Side {(await BlockchainManager.GetSideChainIdAsync()).ToStringChainId()}";
                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsLoaded = false;
            IsCompletelyLoaded = false;
            StateHasChanged();

            TokenInfoWithBalanceList.Clear();

            var sideChainNativeToken = await TokenManager.GetNativeTokenInfoOnSideChainAsync();
            TokenInfoWithBalanceList.Add(new TokenInfoWithBalance()
            {
                IsNative = true,
                Decimals = sideChainNativeToken.Decimals,
                Issuer = sideChainNativeToken.Issuer,
                Symbol = sideChainNativeToken.Symbol,
                TokenName = sideChainNativeToken.TokenName,
                SideChainSupply = sideChainNativeToken.Supply,
                TotalSupply = sideChainNativeToken.TotalSupply,
                IssueChainId = sideChainNativeToken.IssueChainId
            });

            var tokenSymbolList = await TokenManager.GetTokenSymbolsFromStorageAsync();

            foreach (var symbol in tokenSymbolList)
            {
                var sideChainToken = await TokenManager.GetTokenInfoOnSideChainAsync(symbol);

                if (!string.IsNullOrEmpty(sideChainToken.Symbol))
                {
                    var tokenInfo = new TokenInfoWithBalance()
                    {
                        Decimals = sideChainToken.Decimals,
                        Issuer = sideChainToken.Issuer,
                        Symbol = sideChainToken.Symbol,
                        TokenName = sideChainToken.TokenName,
                        SideChainSupply = sideChainToken.Supply,
                        TotalSupply = sideChainToken.TotalSupply,
                        IssueChainId = sideChainToken.IssueChainId
                    };
                    TokenInfoWithBalanceList.Add(tokenInfo);
                }
                else
                {
                    await TokenManager.RemoveTokenSymbolFromStorageAsync(symbol);
                }
            }

            TokenInfoWithBalanceList = TokenInfoWithBalanceList.OrderByDescending(x => x.IsNative).ThenBy(x => x.TokenName).ToList();
            IsLoaded = true;
            StateHasChanged();

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                var mainChainToken = await TokenManager.GetTokenInfoOnMainChainAsync(tokenInfo.Symbol);

                tokenInfo.MainChainSupply = mainChainToken.Supply;
                StateHasChanged();
            }

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
                StateHasChanged();
            }

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                var mainChainGetBalanceOutput = await TokenManager.GetBalanceOnMainChainAsync(tokenInfo.Symbol);
                tokenInfo.MainChainBalance = mainChainGetBalanceOutput.Balance;
                StateHasChanged();
            }

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                var sideChainGetBalanceOutput = await TokenManager.GetBalanceOnSideChainAsync(tokenInfo.Symbol);
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
            await TokenManager.RemoveTokenSymbolFromStorageAsync(tokenInfo.Symbol);
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