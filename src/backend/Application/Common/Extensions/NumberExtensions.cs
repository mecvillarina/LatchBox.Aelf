using AElf;
using System;
using System.Globalization;
using System.Numerics;

namespace Application.Common.Extensions
{
    public static class NumberExtensions
    {
        public static decimal ToAmount(this long value, int decimals)
        {
            return Convert.ToDecimal(((double)value) / Math.Pow(10, decimals));
        }

        public static long ToChainAmount(this decimal value, int decimals)
        {
            return (long)((double)value * Math.Pow(10, decimals));
        }

        public static long ToChainAmount(this double value, int decimals)
        {
            return (long)(value * Math.Pow(10, decimals));
        }

        public static long ToChainAmount(this long value, int decimals)
        {
            return (long)(value * Math.Pow(10, decimals));
        }

        public static int ToChainId(this string chainIdBase58)
        {
            return ChainHelper.ConvertBase58ToChainId(chainIdBase58);
        }

        public static int CountDecimalPlaces(this double value)
        {
            string[] digits = value.ToString().Split(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return digits.Length == 2 ? digits[1].Length : 0;
        }
    }
}
