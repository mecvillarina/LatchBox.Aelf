using AElf.Client.MultiToken;
using Client.Infrastructure.Models;

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
            var resourceTokens = await TokenManager.GetResourceTokenInfoListAsync(_creds.Item1, _creds.Item2);

            TokenInfoWithBalanceList.Add(new TokenInfoWithBalance() { Token = nativeToken });

            foreach (var token in resourceTokens.Value)
            {
                TokenInfoWithBalanceList.Add(new TokenInfoWithBalance() { Token = token });
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
    }
}