using Client.Infrastructure.Models;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IWalletManager : IManager
    {
        Task<WalletInformation> GetWalletInformationAsync();
        Task AuthenticateAsync(string password);
    }
}