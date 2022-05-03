using AElf.Cryptography.ECDSA;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IKeyStore
    {
        Task<ECKeyPair> ReadKeyPairAsync(string filename, string password);
        void SaveKeyStoreJsonContent(string filename, string scrypt);
        void DeleteKeyStore(string filename);
    }
}