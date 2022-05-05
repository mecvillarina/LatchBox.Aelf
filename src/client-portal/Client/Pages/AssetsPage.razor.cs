using Client.Infrastructure.Models;
using Client.Pages.Tokens.Modals;

namespace Client.Pages
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
                        _creds = await WalletManager.GetWalletCrdentialsAsync();
                        await FetchDataAsync();
                    });
                });
            }
        }

        private async Task FetchDataAsync()
        {
            IsCompletelyLoaded = false;

            TokenInfoWithBalanceList.Clear();

            var nativeToken = await TokenManager.GetNativeTokenInfoAsync(_creds.Item1, _creds.Item2);
            TokenInfoWithBalanceList.Add(new TokenInfoWithBalance() { Token = nativeToken });

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

        private async Task InvokeCreateTokenModalAsync()
        {
            var dialog = DialogService.Show<CreateTokenModal>($"Create New Lock");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                await FetchDataAsync();
            }
        }
    }
}