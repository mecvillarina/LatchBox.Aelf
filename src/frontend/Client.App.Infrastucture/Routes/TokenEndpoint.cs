namespace Client.App.Infrastructure.Routes
{
    public static class TokenEndpoint
    {
        public const string GetAllTokens = "api/token/getAllTokens/{0}";
        public const string GetTokenBalances = "api/token/getTokenBalances/{0}?address={1}";
    }
}
