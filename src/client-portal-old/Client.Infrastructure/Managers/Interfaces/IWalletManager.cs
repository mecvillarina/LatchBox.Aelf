using AElf.Cryptography.ECDSA;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IWalletManager : IManager
    {
        Task<string> GetWalletAddressAsync();
        Task<ECKeyPair> GetWalletKeyPairAsync();
        Task<bool> AuthenticateAsync(string password);
    }
}