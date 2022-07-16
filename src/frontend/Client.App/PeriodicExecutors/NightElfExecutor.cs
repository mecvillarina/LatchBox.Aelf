using Client.App.Services;
using System;
using System.Timers;

namespace Client.App.PeriodicExecutors
{
    public class NightElfExecutor : IDisposable
    {
        private readonly NightElfService _nightElfService;

        public NightElfExecutor(NightElfService nightElfService)
        {
            _nightElfService = nightElfService;
        }

        public bool IsConnected { get; set; }

        public event EventHandler Disconnected;

        private Timer _timer;
        private bool _running;

        public void StartExecuting()
        {
            if (!_running)
            {
                _timer = new Timer();
                _timer.Interval = 1000;
                _timer.Elapsed += HandleTimer;
                _timer.AutoReset = true;
                _timer.Enabled = true;
                _running = true;
                HandleTimer(null, null);
            }
        }

        private async void HandleTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                var isConnected = await _nightElfService.IsConnectedAsync();
                if (IsConnected && !isConnected)
                {
                    Disconnected?.Invoke(this, e);
                }
                IsConnected = isConnected;
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

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}
