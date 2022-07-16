using Client.App.Infrastructure.Managers;
using Client.App.Infrastructure.Managers.Interfaces;
using Client.App.Services;
using System;
using System.Linq;
using System.Timers;

namespace Client.App.PeriodicExecutors
{
    public class FetchDataExecutor : IDisposable
    {
        private readonly ChainService _chainService;

        private Timer _timer;
        private bool _running;
        bool _isFetching;

        public FetchDataExecutor(ChainService chainService)
        {
            _chainService = chainService;
        }

        public void StartExecuting()
        {
            if (!_running)
            {
                _timer = new Timer();
                _timer.Interval = 30000;
                _timer.Elapsed += HandleTimer;
                _timer.AutoReset = true;
                _timer.Enabled = true;
                _running = true;
            }
        }

        async void HandleTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                if (_isFetching)
                {
                    return;
                }

                Console.WriteLine($"Fetching...");

                _isFetching = true;

                //#if RELEASE
                await _chainService.GetAllSupportedChainsAsync();
                await _chainService.FetchChainDataAsync();
                //#endif

                Console.WriteLine($"Fetch Done...");
            }
            catch
            {
                Console.WriteLine($"Fetch Data Executor: Fetch Error");
            }
            finally
            {
                _isFetching = false;
            }
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
