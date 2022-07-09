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
        private readonly IChainManager _chainManager;
        private readonly IExceptionHandler _exceptionHandler;

        private Timer _timer;
        private bool _running;
        bool _isFetching;

        public FetchDataExecutor(IExceptionHandler exceptionHandler, IChainManager chainManager)
        {
            _exceptionHandler = exceptionHandler;
            _chainManager = chainManager;
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
                var data = await _chainManager.FetchSupportedChainsAsync();
                if (!data.Any())
                {
                    var chainsResult = await _exceptionHandler.HandlerRequestTaskAsync(() => _chainManager.GetAllSupportedChainsAsync());

                    if (chainsResult.Succeeded)
                    {
                        if (chainsResult.Data.Any())
                        {
                            await _chainManager.SetCurrentChainAsync(chainsResult.Data.First().ChainIdBase58);
                        }
                    }
                }
                else
                {
                    var currentChain = await _chainManager.FetchCurrentChainAsync();
                    if (currentChain == null || !data.Any(x => x.ChainIdBase58 == currentChain))
                    {
                        await _chainManager.SetCurrentChainAsync(data.First().ChainIdBase58);
                    }
                }
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
