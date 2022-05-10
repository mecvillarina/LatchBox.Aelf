using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class WalletManager : ManagerBase, IWalletManager
    {
        private readonly IAccountsService _accountsService;

        public WalletManager(IManagerToolkit managerToolkit, IAccountsService accountsService) : base(managerToolkit)
        {
            _accountsService = accountsService;
        }

        public async Task<WalletInformation> GetWalletInformationAsync()
        {
            return await ManagerToolkit.GetWalletAsync();
        }

        public async Task<(WalletInformation, string)> GetWalletCredentialsAsync()
        {
            var wallet = await ManagerToolkit.GetWalletAsync();

            if (wallet == null) throw new ConnectWalletException("Connect your wallet first.");

            var pass = _accountsService.FetchKeyStorePassword(wallet.Filename);
            return (wallet, pass);
        }

        public async Task AuthenticateAsync(string password)
        {
            var wallet = await GetWalletInformationAsync();

            if (wallet == null) throw new ConnectWalletException("Connect your wallet first.");

            await _accountsService.GetAccountKeyPairAsync(wallet.Filename, password);
        }

    }
}
