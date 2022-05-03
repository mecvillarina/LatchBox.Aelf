using AElf.Cryptography;
using AElf.Cryptography.ECDSA;
using AElf.Cryptography.Exceptions;
using AElf.Types;
using Client.Infrastructure.Services.Interfaces;
using Nethereum.KeyStore;
using Nethereum.KeyStore.Crypto;
using System.IO;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services
{
    public class AElfKeyStore : IKeyStore
    {
        private const string KeyFileExtension = ".json";
        private const string KeyFolderName = "keys";

        private readonly KeyStoreService _keyStoreService;

        public AElfKeyStore()
        {
            _keyStoreService = new KeyStoreService();
            CreateKeystoreDirectory();
        }

        public void SaveKeyStoreJsonContent(string filename, string scrypt)
        {
            var fullPath = GetKeyFileFullPath(filename);

            using (var writer = File.CreateText(fullPath))
            {
                writer.Write(scrypt);
                writer.Flush();
            }
        }

        public void DeleteKeyStore(string filename)
        {
            var fullPath = GetKeyFileFullPath(filename);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<ECKeyPair> ReadKeyPairAsync(string filename, string password)
        {
            try
            {
                var keyFilePath = GetKeyFileFullPath(filename);
                var privateKey = await Task.Run(() =>
                {
                    using (var textReader = File.OpenText(keyFilePath))
                    {
                        var json = textReader.ReadToEnd();
                        return _keyStoreService.DecryptKeyStoreFromJson(password, json);
                    }
                });

                return CryptoHelper.FromPrivateKey(privateKey);
            }

            catch (FileNotFoundException ex)
            {
                throw new KeyStoreNotFoundException("Keystore file not found.", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new KeyStoreNotFoundException("Invalid keystore path.", ex);
            }
            catch (DecryptionException ex)
            {
                throw new InvalidPasswordException("Invalid password.", ex);
            }
        }

        private string GetKeyFileFullPath(string filename)
        {
            var path = GetKeyFileFullPathStrict(filename);
            return File.Exists(path) ? path : GetKeyFileFullPathStrict(filename);
        }

        private string GetKeyFileFullPathStrict(string filename)
        {
            var dirPath = GetKeystoreDirectoryPath();
            var filePath = Path.Combine(dirPath, filename);
            var filePathWithExtension = Path.ChangeExtension(filePath, KeyFileExtension);
            return filePathWithExtension;
        }

        private DirectoryInfo CreateKeystoreDirectory()
        {
            var dirPath = GetKeystoreDirectoryPath();
            return Directory.CreateDirectory(dirPath);
        }

        private string GetKeystoreDirectoryPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "..", KeyFolderName);
        }
    }
}
