namespace Client.App.Infrastructure.Routes
{
    public static class TokenEndpoint
    {
        public const string GetAllTokens = "api/token/{0}/getAllTokens";
        public const string GetTokenBalances = "api/token/{0}/getTokenBalances?address={1}";
    }
}
