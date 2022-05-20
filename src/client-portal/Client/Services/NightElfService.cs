using Microsoft.JSInterop;

namespace Client.Services
{
    public class NightElfService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private bool jsLoaded;

        //public static event Func<Task>? ConnectEvent;
        //public static event Func<Task>? DisconnectEvent;

        public NightElfService(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => LoadScripts(jsRuntime).AsTask());
        }

        public ValueTask<IJSObjectReference> LoadScripts(IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/aelfJsInterop.js");
        }

        public async ValueTask<bool> HasNightElfAsync()
        {
            var module = await _moduleTask.Value;

            var result = await module.InvokeAsync<bool>("HasNightElf");
            return result;
        }

        public async ValueTask InitializeNightElfAsync(string appName, string nodeUrl)
        {
            var module = await _moduleTask.Value;

            if (!jsLoaded)
            {
                await module.InvokeVoidAsync("loadJs", "https://unpkg.com/aelf-sdk@3.2.40/dist/aelf.umd.js");
                jsLoaded = true;
            }

            await module.InvokeVoidAsync("Initialize", nodeUrl, appName);
        }

        public async ValueTask<bool> IsConnectedAsync()
        {
            var module = await _moduleTask.Value;
            var result = await module.InvokeAsync<bool>("IsConnected");
            Console.WriteLine($"Connected: {result}");
            return result;

        }

        public async ValueTask<string?> LoginAsync()
        {
            var module = await _moduleTask.Value;
            var result = await module.InvokeAsync<string?>("Login");

            Console.WriteLine($"Login: {result}");
            return string.IsNullOrEmpty(result) ? null : result;
        }

        public async ValueTask<string?> GetAddressAsync()
        {
            var module = await _moduleTask.Value;
            var result = await module.InvokeAsync<string?>("GetAddress");
            Console.WriteLine($"GetAddress: {result}");
            return string.IsNullOrEmpty(result) ? null : result;
        }
        public async ValueTask LogoutAsync()
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("Logout");
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }

    }
}
