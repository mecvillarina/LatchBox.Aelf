using System;
using System.Globalization;
using System.Numerics;

namespace Client.Infrastructure.Extensions
{
    public static class NumberExtensions
    {
        public static decimal ToAmount(this long value, int decimals)
        {
            return Convert.ToDecimal(((double)value) / Math.Pow(10, decimals));
        }

        public static decimal ToAmount(this long value, byte decimals)
        {
            return Convert.ToDecimal(((double)value) / Math.Pow(10, decimals));
        }

        public static BigInteger ToAmount(this decimal value, int decimals)
        {
            return (long)((double)value * Math.Pow(10, decimals));
        }

        public static BigInteger ToAmount(this double value, int decimals)
        {
            return (long)(value * Math.Pow(10, decimals));
        }

        public static int CountDecimalPlaces(this double value)
        {
            string[] digits = value.ToString().Split(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return digits.Length == 2 ? digits[1].Length : 0;
        }
    }
}
