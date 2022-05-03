using Blazored.LocalStorage;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class ManagerToolkit : IManagerToolkit
    {
        private readonly ILocalStorageService _localStorageService;
        public AElfSettings AelfSettings { get; }

        public string FilePathTemp => Path.Combine(Directory.GetCurrentDirectory(), "..", "temp");
        public string FilePathWallet => Path.Combine(Directory.GetCurrentDirectory(), "..", "wallet");

        public ManagerToolkit(ILocalStorageService localStorageService, IOptions<AElfSettings> aelfSettingsOption)
        {
            _localStorageService = localStorageService;
            AelfSettings = aelfSettingsOption.Value;

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

        public async Task SaveWalletAsync(string filename, string address)
        {
            var wallet = new WalletInformation()
            {
                Filename = filename,
                Address = address
            };

            await _localStorageService.SetItemAsync(StorageConstants.Local.Wallet, wallet);
        }

        public async Task<WalletInformation> GetWalletAsync()
        {
            var isExists = await _localStorageService.ContainKeyAsync(StorageConstants.Local.Wallet);

            if (!isExists) return null;

            return await _localStorageService.GetItemAsync<WalletInformation>(StorageConstants.Local.Wallet);
        }

        public async Task ClearLocalStorageAsync()
        {
            await _localStorageService.ClearAsync();
        }
    }
}
