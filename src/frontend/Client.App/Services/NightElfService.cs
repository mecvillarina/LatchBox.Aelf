using AElf.Types;
using Application.Common.Dtos;
using Client.App.SmartContractDto;
using Microsoft.JSInterop;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class NightElfService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private bool jsLoaded;

        public NightElfService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => LoadScripts(jsRuntime).AsTask());
        }

        public ValueTask<IJSObjectReference> LoadScripts(IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/aelfJsInterop.js");
        }

        public async ValueTask<bool> HasNightElfAsync()
        {
            var module = await moduleTask.Value;

            var result = await module.InvokeAsync<bool>("HasNightElf");
            Console.WriteLine($"Installed: {result}");
            return result;
        }

        public async ValueTask InitializeNightElfAsync(string appName, string nodeUrl = "https://explorer-test.aelf.io/chain")
        {
            var module = await moduleTask.Value;

            if (!jsLoaded)
            {
                await module.InvokeVoidAsync("loadJs", "https://unpkg.com/aelf-sdk@3.2.40/dist/aelf.umd.js");
                jsLoaded = true;
            }

            await module.InvokeVoidAsync("Initialize", nodeUrl, appName);
        }

        public async ValueTask<TransactionResultDto> SendTxAsync(string address, string functionName, object payload)
        {
            var module = await moduleTask.Value;
            var txId = await module.InvokeAsync<string>("SendTx", address, functionName, payload);

            if (txId == null) return null;

            var txResult = await GetTxStatus(txId);
            var i = 0;
            while (i < 10)
            {
                if(txResult.Status == null && txResult.ErrorMessage != null)
                {
                    break;
                }

                if (txResult.Status == TransactionResultStatus.Mined.ToString().ToUpper())
                {
                    break;
                }

                if (txResult.Status == TransactionResultStatus.Failed.ToString().ToUpper() || txResult.Status == TransactionResultStatus.NodeValidationFailed.ToString().ToUpper())
                {
                    break;
                }

                await Task.Delay(1000);
                txResult = await GetTxStatus(txId);
                i++;
            }

            return txResult;
        }

        public async ValueTask<T> ReadSmartContractAsync<T>(string address, string functionName, ExpandoObject payload)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<T>("ReadSmartContract", address, functionName, payload);
        }

        public async ValueTask<bool> IsConnectedAsync()
        {
            var module = await moduleTask.Value;
            var result = await module.InvokeAsync<bool>("IsConnected");
            Console.WriteLine($"Connected: {result}");
            return result;

        }

        public async ValueTask<bool> LoginAsync()
        {
            var module = await moduleTask.Value;
            var result = await module.InvokeAsync<bool>("Login");
            Console.WriteLine($"Login: {result}");
            return result;
        }

        public async ValueTask<string?> GetAddressAsync()
        {
            var module = await moduleTask.Value;
            var result = await module.InvokeAsync<string?>("GetAddress");
            Console.WriteLine($"GetAddress: {result}");
            return string.IsNullOrEmpty(result) ? null : result;
        }

        public async ValueTask LogoutAsync()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("Logout");
        }

        //public async ValueTask<BalanceResult?> GetBalanceAsync()
        //{
        //    var module = await moduleTask.Value;
        //    var result = await module.InvokeAsync<BalanceResult?>("GetBalance");
        //    return result;
        //}

        public async ValueTask<TransactionResultDto> GetTxStatus(string txId)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<TransactionResultDto>("GetTxStatus", txId);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
