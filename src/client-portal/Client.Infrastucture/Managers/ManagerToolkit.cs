using Blazored.LocalStorage;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Client.Infrastucture.Enums;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class ManagerToolkit : IManagerToolkit
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IAuthTokenService _authTokenService;
        private readonly IBlockChainService _blockChainService;

        public AElfSettings AelfSettings { get; }

        public string FilePathTemp => Path.Combine(Directory.GetCurrentDirectory(), "..", "temp");
        public string FilePathWallet => Path.Combine(Directory.GetCurrentDirectory(), "..", "wallet");

        public ManagerToolkit(ILocalStorageService localStorageService, IOptions<AElfSettings> aelfSettingsOption, IAuthTokenService authTokenService, IBlockChainService blockChainService)
        {
            _localStorageService = localStorageService;
            AelfSettings = aelfSettingsOption.Value;
            _authTokenService = authTokenService;
            _blockChainService = blockChainService;

            Init();
        }

        private void Init()
        {
            if (!Directory.Exists(FilePathTemp))
            {
                Directory.CreateDirectory(FilePathTemp);
            }

            if (!Directory.Exists(FilePathWallet))
            {
                Directory.CreateDirectory(FilePathWallet);
            }
        }

        public async Task SaveWalletAuthHandlerAsync(AuthTokenHandler tokenHandler, string address)
        {
            var wallet = new WalletAuthHandler()
            {
                TokenHandler = tokenHandler,
                Address = address
            };

            await _localStorageService.SetItemAsync(StorageConstants.Local.Wallet, wallet);
        }

        public async Task<WalletAuthHandler> GetWalletAsync()
        {
            var isExists = await _localStorageService.ContainKeyAsync(StorageConstants.Local.Wallet);

            if (!isExists) return null;

            var wallet = await _localStorageService.GetItemAsync<WalletAuthHandler>(StorageConstants.Local.Wallet);

            if (!wallet.TokenHandler.IsValid()) return null;

            //validate
            var authTokenResult = _authTokenService.ValidateToken(wallet.TokenHandler.Token);
            if (authTokenResult.Status != AuthTokenStatus.Valid) return null;

            var currentChainId = await _blockChainService.GetChainIdAsync();
            var chainIdClaimIdentifier = authTokenResult.Principal.Claims.FirstOrDefault(x => x.Type == "ChainId");
            if (chainIdClaimIdentifier == null || chainIdClaimIdentifier.Value != currentChainId.ToString()) return null;

            var nodeClaimIdentifier = authTokenResult.Principal.Claims.FirstOrDefault(x => x.Type == "Node");
            if (nodeClaimIdentifier == null || nodeClaimIdentifier.Value != AelfSettings.Node) return null;

            return wallet;
        }

        public async Task ClearAccountLocalStorageAsync()
        {
            await _localStorageService.RemoveItemsAsync(new List<string>() { StorageConstants.Local.Wallet });
        }
    }
}
