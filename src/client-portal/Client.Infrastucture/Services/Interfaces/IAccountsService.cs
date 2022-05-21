using AElf.Cryptography.ECDSA;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IAccountsService
    {
        void SaveKeyStoreJsonContent(string filename, string content);
        void RemoveKeyStore(string filename);
        Task<ECKeyPair> GetAccountKeyPairFromFileAsync(string filename, string password);
        byte[] Sign(ECKeyPair keyPair, byte[] data);
    }
}