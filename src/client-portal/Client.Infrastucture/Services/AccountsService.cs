using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using Client.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
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

        public void SaveKeyStorePassJsonContent(string keyStoreFilename, string password)
        {
            var obj = new JObject();
            obj["p"] = password;

            var content = JsonConvert.SerializeObject(obj);
            var filename = keyStoreFilename.Replace(".json", "_pass.json");
            _keyStore.SaveKeyStoreJsonContent(filename, content);
        }

        public void RemoveKeyStore(string filename)
        {
            _keyStore.DeleteKeyStore(filename);
        }

        public string FetchKeyStorePassword(string keyStoreFilename)
        {
            var filename = keyStoreFilename.Replace(".json", "_pass.json");
            var content = _keyStore.FetchKeyStoreContent(filename);
            var jsonObj = JsonConvert.DeserializeObject<JObject>(content);
            return jsonObj["p"].ToString();
        }

        public async Task<ECKeyPair> GetAccountKeyPairAsync(string filename, string password)
        {
            return await _keyStore.ReadKeyPairAsync(filename, password);
        }

        public async Task<byte[]> SignAsync(string keyStoreFile, string password, byte[] data)
        {
            var signature = CryptoHelper.SignWithPrivateKey((await GetAccountKeyPairAsync(keyStoreFile, password)).PrivateKey, data);
            return signature;
        }

    }
}
