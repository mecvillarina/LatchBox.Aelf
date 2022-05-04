using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface ITokenManager : IManager
    {
        Task<TokenInfo> GetNativeTokenInfoAsync(WalletInformation wallet, string password);
        Task<TokenInfoList> GetResourceTokenInfoListAsync(WalletInformation wallet, string password);
        Task<TokenInfo> GetTokenInfoAsync(WalletInformation wallet, string password, string symbol);
        Task<GetBalanceOutput> GetBalanceAsync(WalletInformation wallet, string password, string symbol);
        Task CreateTokenAsync(WalletInformation wallet, string password);
    }
}