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

        public List<TokenInfoWithBalance> TokenInfoWithBalanceList { get; set; } = new();
        private (WalletInformation, string) _creds;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await PageService.EnsureAuthenticatedAsync(async (authenticated) =>
                {
                    if (!authenticated) return;

                    await InvokeAsync(async () =>
                    {
                        _creds = await WalletManager.GetWalletCredentialsAsync();
                        try
                        {
                            await TokenManager.CreateAsync(_creds.Item1, _creds.Item2, "LATCH", "LATCH", 300000000_00000000, 8, true);
                            await TokenManager.IssueAsync(_creds.Item1, _creds.Item2, "LATCH", 10000000_00000000, "MINT1", _creds.Item1.Address);
                        }
                        catch
                        {

                        }
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

            var nativeToken = await TokenManager.GetNativeTokenInfoAsync(_creds.Item1, _creds.Item2);
            TokenInfoWithBalanceList.Add(new TokenInfoWithBalance() { Token = nativeToken, IsNative = true });

            var tokenSymbolList = await TokenManager.GetTokenSymbolsFromStorageAsync();

            foreach (var symbol in tokenSymbolList)
            {
                var token = await TokenManager.GetTokenInfoAsync(_creds.Item1, _creds.Item2, symbol);

                if (!string.IsNullOrEmpty(token.Symbol))
                {
                    TokenInfoWithBalanceList.Add(new TokenInfoWithBalance() { Token = token });
                }
                else
                {
                    await TokenManager.RemoveTokenSymbolFromStorageAsync(symbol);
                }
            }

            IsLoaded = true;
            StateHasChanged();

            foreach (var tokenInfo in TokenInfoWithBalanceList)
            {
                var getBalanceOutput = await TokenManager.GetBalanceAsync(_creds.Item1, _creds.Item2, tokenInfo.Token.Symbol);
                tokenInfo.Balance = getBalanceOutput.Balance;
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
            await TokenManager.RemoveTokenSymbolFromStorageAsync(tokenInfo.Token.Symbol);
        }

        private async Task InvokeIssueTokenModalAsync(TokenInfoWithBalance tokenInfo)
        {
            var parameters = new DialogParameters()
            {
                { nameof(IssueTokenModal.Model), new IssueTokenParameter() { Symbol = tokenInfo.Token.Symbol, TokenName = tokenInfo.Token.TokenName, Decimals = tokenInfo.Token.Decimals } }
            };

            var dialog = DialogService.Show<IssueTokenModal>($"Issue New Token", parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }
    }
}