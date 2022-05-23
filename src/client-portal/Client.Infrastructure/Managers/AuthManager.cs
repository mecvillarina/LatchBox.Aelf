using AElf;
using AElf.Types;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class AuthManager : ManagerBase, IAuthManager
    {
        private readonly IAccountsService _accountsService;
        private readonly IAuthTokenService _authTokenService;
        private readonly IBlockChainService _blockChainService;
        private readonly IBlockchainManager _blockchainManager;
        public AuthManager(IManagerToolkit managerToolkit, IAccountsService accountsService, IAuthTokenService authTokenService, IBlockChainService blockChainService, IBlockchainManager blockchainManager) : base(managerToolkit)
        {
            _accountsService = accountsService;
            _authTokenService = authTokenService;
            _blockChainService = blockChainService;
            _blockchainManager = blockchainManager;
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
                    var keyPair = await _accountsService.GetAccountKeyPairFromFileAsync(filename, password);

                    var chainId = _blockchainManager.GetSideChainId();
                    var node = ManagerToolkit.AelfSettings.SideChainNode;
                    var claims = new Dictionary<string, string>()
                    {
                        { "PrivateKey", keyPair.PrivateKey.ToHex() },
                        { "PublicKey", keyPair.PublicKey.ToHex() },
                        { "ChainId", chainId.ToString() },
                        { "Node", node },
                        { "Password", password },
                    };

                    _accountsService.RemoveKeyStore(filename);
                    var tokenHandler = _authTokenService.GenerateToken(claims);
                    var address = Address.FromPublicKey(keyPair.PublicKey);
                    await ManagerToolkit.SaveWalletAuthHandlerAsync(tokenHandler, address.ToBase58());
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

            if (wallet != null)
            {
                await ManagerToolkit.ClearAccountLocalStorageAsync();
            }
        }

        public async Task DisconnectWalletAsync()
        {
            await ClearKeyStoreAsync();
            await ManagerToolkit.ClearAccountLocalStorageAsync();
        }
    }
}
