﻿using Microsoft.JSInterop;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class NightElfService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private bool jsLoaded;

        //public static event Func<Task>? ConnectEvent;
        //public static event Func<Task>? DisconnectEvent;

        public NightElfService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => LoadScripts(jsRuntime).AsTask());
        }

        public ValueTask<IJSObjectReference> LoadScripts(IJSRuntime jsRuntime)
        {
            //await jsRuntime.InvokeAsync<IJSObjectReference>("import", "https://unpkg.com/aelf-sdk@3.2.40/dist/aelf.umd.js");
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

        public async ValueTask<string> ExecuteSmartContractAsync(string address, string functionName, ExpandoObject payload)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("ExecuteSmartContract", address, functionName, payload);
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
        public async ValueTask<string?> LoginAsync()
        {
            var module = await moduleTask.Value;
            var result = await module.InvokeAsync<string?>("Login");

            Console.WriteLine($"Login: {result}");
            return string.IsNullOrEmpty(result) ? null : result;
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

        //public async ValueTask<TransactionStatusResult> GetTxStatus(string txId)
        //{
        //    var module = await moduleTask.Value;
        //    return await module.InvokeAsync<TransactionStatusResult>("GetTxStatus", txId);
        //}

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
