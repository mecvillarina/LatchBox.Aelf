using Client.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Client.Services
{
    public class AppHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private volatile bool _ready = false;
        private readonly IBlockChainService _blockChainService;
        private readonly IMemoryCache _cache;
        private Timer _timer;

        public AppHostedService(IServiceProvider services, IHostApplicationLifetime lifetime, IBlockChainService blockchainService, IMemoryCache cache)
        {
            _services = services;
            lifetime.ApplicationStarted.Register(() => _ready = true);
            _blockChainService = blockchainService;
            _cache = cache;
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }


        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_ready)
            {
                await Task.Delay(1000);
            }

            _timer = new Timer(FetchDataAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private void FetchDataAsync(object state)
        {
            var mainChainStatus = _blockChainService.GetMainChainStatusAsync().Result;

            if (mainChainStatus != null)
            {
                _cache.Set("MainChainStatus", mainChainStatus);
            }

            var sideChainStatus = _blockChainService.GetSideChainStatusAsync().Result;

            if (sideChainStatus != null)
            {
                _cache.Set("SideChainStatus", sideChainStatus);
            }
        }
    }
}
