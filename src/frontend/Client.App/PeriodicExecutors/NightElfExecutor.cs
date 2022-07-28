using Client.App.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Client.App.PeriodicExecutors
{
    public class NightElfExecutor : IAsyncDisposable
    {
        private readonly NightElfService _nightElfService;

        private Task? _timerTask;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cts = new();
        public NightElfExecutor(NightElfService nightElfService)
        {
            _nightElfService = nightElfService;
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(2000));

        }

        public bool HasConnected { get; set; }

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public void StartExecuting()
        {
            _timerTask = DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {
            try
            {
                while(await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    var isConnected = await _nightElfService.IsConnectedAsync();
                    if (HasConnected && !isConnected)
                    {
                        InvokeDisconnect();
                    }

                    if (!HasConnected && isConnected)
                    {
                        Connected?.Invoke(this, EventArgs.Empty);
                    }

                    HasConnected = isConnected;
                }
            }
            catch
            {
                Console.WriteLine($"NightElfExecutor: Error");
            }
        }

        public void InvokeDisconnect()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
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
