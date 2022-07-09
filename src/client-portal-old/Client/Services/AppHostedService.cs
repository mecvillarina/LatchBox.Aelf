using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Client.Services
{
    public class AppHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private volatile bool _ready = false;
        private readonly IBlockChainService _blockChainService;
        private readonly IMemoryCache _cache;
        private readonly AElfSettings _aelfSettings;
        private Timer _timer;

        public AppHostedService(IServiceProvider services, IHostApplicationLifetime lifetime, IBlockChainService blockchainService, IMemoryCache cache, IOptions<AElfSettings> _aelfSettingsOptions)
        {
            _services = services;
            _blockChainService = blockchainService;
            _cache = cache;
            _aelfSettings = _aelfSettingsOptions.Value;

            lifetime.ApplicationStarted.Register(() => _ready = true);
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

            _timer = new Timer(FetchDataAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
        }

        private void FetchDataAsync(object state)
        {
            if (_ready)
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

                var mainChainTokenContractAddress = _blockChainService.GetMainChainContractAddressAsync(_aelfSettings.MainChainMultiTokenContractAddress).Result;

                if (mainChainTokenContractAddress != null)
                {
                    _cache.Set("MainChainTokenAddress", mainChainTokenContractAddress);
                }

                var sideChainTokenContractAddress = _blockChainService.GetSideChainContractAddressAsync(_aelfSettings.SideChainMultiTokenContractAddress).Result;

                if (sideChainTokenContractAddress != null)
                {
                    _cache.Set("SideChainTokenAddress", sideChainTokenContractAddress);
                }
            }
        }
    }
}
