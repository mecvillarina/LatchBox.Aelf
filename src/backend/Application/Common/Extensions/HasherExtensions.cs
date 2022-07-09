using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Models;

namespace Application.Common.Extensions
{
    public static class HasherExtensions
    {
        public static HashSaltResult Hash(string content, int saltLength, HashAlgorithm hashAlgo)
        {
            var rng = new CryptographicExtensions();
            var saltBytes = rng.GenerateRandomCryptographicBytes(saltLength);
            var passwordAsBytes = Encoding.UTF8.GetBytes(content);
            var passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);
            var digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
            return new HashSaltResult(Convert.ToBase64String(saltBytes), Convert.ToBase64String(digestBytes));
        }

        public static HashSaltResult Hash(string content, string salt, HashAlgorithm hashAlgo)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordAsBytes = Encoding.UTF8.GetBytes(content);
            var passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);
            var digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
            return new HashSaltResult(Convert.ToBase64String(saltBytes), Convert.ToBase64String(digestBytes));
        }
    }
}