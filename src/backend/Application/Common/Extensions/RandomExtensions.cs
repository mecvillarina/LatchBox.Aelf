using System;
using System.Linq;

namespace Application.Common.Extensions
{
    public static class RandomExtensions
    {
        private readonly static Random _random = new Random(DateTime.UtcNow.Millisecond);
        public static string GenerateAlphaNumericCharacters(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GenerateNumericCharacter(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GenerateSalt(int length)
        {
            string SmallLetters = "qwertyuiopasdfghjklzxcvbnm";
            string Digits = "0123456789";
            string AllChar = SmallLetters + Digits;

            return new string(Enumerable.Repeat(AllChar, length)
             .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GeneratePassword(int length)
        {
            string CapitalLetters = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string SmallLetters = "qwertyuiopasdfghjklzxcvbnm";
            string Digits = "0123456789";
            string SpecialCharacters = "!@#$%^&*()-_=+<,>.";
            string AllChar = CapitalLetters + SmallLetters + Digits + SpecialCharacters;

            return new string(Enumerable.Repeat(AllChar, length)
             .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
