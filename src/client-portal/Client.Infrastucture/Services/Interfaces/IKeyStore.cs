using AElf.Cryptography.ECDSA;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IKeyStore
    {
        Task<ECKeyPair> ReadKeyPairAsync(string json, string password);
        Task<ECKeyPair> ReadKeyPairFromFileAsync(string filename, string password);
        void SaveKeyStoreJsonContent(string filename, string scrypt);
        void DeleteKeyStore(string filename);
        string FetchKeyStoreContent(string filename);
    }
}