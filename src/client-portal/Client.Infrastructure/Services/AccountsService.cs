using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using Client.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IKeyStore _keyStore;

        public AccountsService(IKeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public void SaveKeyStoreJsonContent(string filename, string content)
        {
            _keyStore.SaveKeyStoreJsonContent(filename, content);
        }

        public void RemoveKeyStore(string filename)
        {
            _keyStore.DeleteKeyStore(filename);
        }

        public async Task<ECKeyPair> GetAccountKeyPairFromFileAsync(string filename, string password)
        {
            return await _keyStore.ReadKeyPairFromFileAsync(filename, password);
        }

        public byte[] Sign(ECKeyPair keyPair, byte[] data)
        {
            var signature = CryptoHelper.SignWithPrivateKey(keyPair.PrivateKey, data);
            return signature;
        }

    }
}
