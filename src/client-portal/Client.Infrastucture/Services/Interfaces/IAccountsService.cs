﻿using AElf.Cryptography.ECDSA;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IAccountsService
    {
        void SaveKeyStoreJsonContent(string filename, string content);
        void RemoveKeyStore(string filename);
        Task<ECKeyPair> GetAccountKeyPairAsync(string filename, string password);
        Task<byte[]> SignAsync(string keyStoreFile, string password, byte[] data);
    }
}