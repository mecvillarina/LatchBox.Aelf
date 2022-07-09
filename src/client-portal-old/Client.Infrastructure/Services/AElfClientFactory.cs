using AElf.Client.Service;
using Client.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace Client.Infrastructure.Services
{
    public class AElfClientFactory
    {
        private AElfClient _mainChainNodeClient;
        private AElfClient _sideChainNodeClient;
        private readonly AElfSettings _settings;
        public AElfClientFactory(IOptions<AElfSettings> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        public AElfClient CreateSideChainNodeClient()
        {
            if (_sideChainNodeClient == null)
            {
                _sideChainNodeClient = new AElfClient(_settings.SideChainNode);
            }

            return _sideChainNodeClient;
        }

        public AElfClient CreateMainChainNodeClient()
        {
            if (_mainChainNodeClient == null)
            {
                _mainChainNodeClient = new AElfClient(_settings.MainChainNode);
            }

            return _mainChainNodeClient;
        }
    }
}
