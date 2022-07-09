using System;
using System.Security.Cryptography;

namespace Application.Common.Extensions
{
    public class CryptographicExtensions
    {
        public string GenerateRandomCryptographicKey(int keyLength)
        {
            return Convert.ToBase64String(GenerateRandomCryptographicBytes(keyLength));
        }

        public byte[] GenerateRandomCryptographicBytes(int keyLength)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return randomBytes;
        }
    }
}