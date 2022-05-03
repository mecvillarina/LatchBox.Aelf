using Client.Infrastructure.Models;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IManagerToolkit : IManager
    {
        AElfSettings AelfSettings { get; }

        string FilePathTemp { get; }
        string FilePathWallet { get; }

        Task SaveWalletAsync(string filename, string address);
        Task<WalletInformation> GetWalletAsync();
        Task ClearLocalStorageAsync();
    }
}