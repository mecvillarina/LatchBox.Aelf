namespace Client.App.Infrastructure.Routes
{
    public static class CrossChainOperationEndpoints
    {
        public const string Create = "api/crosschain/operation/create";
        public const string GetPending = "api/crosschain/operation/pending?from={0}&issueChainId={1}&contractName={2}";
        public const string Confirm = "api/crosschain/operation/confirm";
    }
}
