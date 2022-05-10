using AElf.Client.Service;
using Client.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace Client.Infrastructure.Services
{
    public class AElfClientFactory
    {
        private AElfClient _client;
        private readonly AElfSettings _settings;
        public AElfClientFactory(IOptions<AElfSettings> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        public AElfClient CreateClient()
        {
            if (_client == null)
            {
                _client = new AElfClient(_settings.Node);
            }

            return _client;
        }
    }
}
