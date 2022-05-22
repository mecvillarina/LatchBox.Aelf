using AElf;
using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class WalletManager : ManagerBase, IWalletManager
    {
        private readonly IAccountsService _accountsService;
        private readonly IAuthTokenService _authTokenService;
        public WalletManager(IManagerToolkit managerToolkit, IAccountsService accountsService, IAuthTokenService authTokenService) : base(managerToolkit)
        {
            _accountsService = accountsService;
            _authTokenService = authTokenService;
        }

        private async Task<WalletAuthHandler> GetWalletAsync()
        {
            var wallet = await ManagerToolkit.GetWalletAsync();

            if (wallet == null) throw new ConnectWalletException("Connect your wallet first.");

            return wallet;
        }

        public async Task<string> GetWalletAddressAsync()
        {
            var wallet = await GetWalletAsync();
            return wallet.Address;
        }

        public async Task<ECKeyPair> GetWalletKeyPairAsync()
        {
            var wallet = await GetWalletAsync();
            var authTokenResult = _authTokenService.ValidateToken(wallet.TokenHandler.Token);

            var privateKeyIdentifier = authTokenResult.Principal.Claims.First(x => x.Type == "PrivateKey");
            var privateKey = ByteArrayHelper.HexStringToByteArray(privateKeyIdentifier.Value);

            return CryptoHelper.FromPrivateKey(privateKey);
        }

        public async Task<bool> AuthenticateAsync(string password)
        {
            var wallet = await GetWalletAsync();
            var authTokenResult = _authTokenService.ValidateToken(wallet.TokenHandler.Token);

            var identifier = authTokenResult.Principal.Claims.First(x => x.Type == "Password");

            return identifier.Value == password;
        }

    }
}
