namespace Client.App.Infrastructure.Routes
{
    public static class Server
    {
#if DEBUG
        public const string ApiBaseAddress = "http://localhost:7071";
#elif RELEASE
        public const string ApiBaseAddress = "https://func-latchbox-aelf-testnet-001.azurewebsites.net";
#endif

    }
}