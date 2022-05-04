using Client.Infrastructure.Models;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface ITokenManager : IManager
    {
        Task GetBalanceAsync(WalletInformation wallet, string password, string symbol);
        Task GetTokenInfoAsync(WalletInformation wallet, string password, string symbol);
        Task CreateTokenAsync(WalletInformation wallet, string password);
    }
}