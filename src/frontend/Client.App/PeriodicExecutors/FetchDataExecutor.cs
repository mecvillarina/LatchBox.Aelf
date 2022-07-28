using Client.App.Infrastructure.Managers;
using Client.App.Infrastructure.Managers.Interfaces;
using Client.App.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace Client.App.PeriodicExecutors
{
    public class FetchDataExecutor : IAsyncDisposable
    {
        private readonly ChainService _chainService;

        private Task? _timerTask;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cts = new();

        public FetchDataExecutor(ChainService chainService)
        {
            _chainService = chainService;
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(30000));
        }

        public void StartExecuting()
        {
            _timerTask = DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    try
                    {
                        //#if RELEASE
                        await _chainService.GetAllSupportedChainsAsync();
                        await _chainService.FetchChainDataAsync();
                        //#endif

                        Console.WriteLine($"Fetch Done...");
                    }
                    catch
                    {
                        Console.WriteLine($"Fetch Data Executor: Fetch Error.");
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Fetch Data Executor: Error..");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_timerTask == null) return;

            _cts.Cancel();
            await _timerTask;
            _cts.Dispose();
        }
    }
}
