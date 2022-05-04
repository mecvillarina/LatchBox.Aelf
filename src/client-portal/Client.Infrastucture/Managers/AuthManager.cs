using AElf.Types;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class AuthManager : ManagerBase, IAuthManager
    {
        private readonly IAccountsService _accountsService;

        public AuthManager(IManagerToolkit managerToolkit, IAccountsService accountsService) : base(managerToolkit)
        {
            _accountsService = accountsService;
        }

        public async Task<bool> IsAuthenticated()
        {
            var wallet = await ManagerToolkit.GetWalletAsync();
            return wallet != null;
        }

        public async Task ConnectWalletAsync(IBrowserFile file, string password)
        {
            string filename = $"{Guid.NewGuid()}.json";
            var tempFullPath = $"{ManagerToolkit.FilePathTemp}/{filename}";

            using (MemoryStream ms = new())
            {
                await file.OpenReadStream().CopyToAsync(ms);
                File.WriteAllBytes(tempFullPath, ms.ToArray());
            }

            using (var textReader = File.OpenText(tempFullPath))
            {
                try
                {
                    var content = textReader.ReadToEnd();
                    _accountsService.SaveKeyStoreJsonContent(filename, content);
                    var keyPair = await _accountsService.GetAccountKeyPairAsync(filename, password);
                    _accountsService.SaveKeyStorePassJsonContent(filename, password);
                    var address = Address.FromPublicKey(keyPair.PublicKey);
                    await ManagerToolkit.SaveWalletAsync(filename, address.ToBase58());
                }
                catch
                {
                    _accountsService.RemoveKeyStore(filename);
                    throw;
                }
            }
        }

        public async Task ClearKeyStoreAsync()
        {
            var wallet = await ManagerToolkit.GetWalletAsync();

            if(wallet != null)
            {
                var filename = wallet.Filename;
                _accountsService.RemoveKeyStore(filename);
                _accountsService.RemoveKeyStore(filename.Replace(".json", "_pass.json"));
            }
        }

        public async Task DisconnectWalletAsync()
        {
            await ClearKeyStoreAsync();
            await ManagerToolkit.ClearLocalStorageAsync();
        }
    }
}
