using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Numerics;

namespace Application.Common.Extensions
{
    public static class CryptoUnitConversionExtension
    {
        public static double ToTFuel(this string hexValue)
        {
            hexValue ??= "0x0";

            BigInteger bigIntVal = hexValue.HexToBigInteger(false);
            var value = (double)bigIntVal;
            return value / Math.Pow(10, 18);
        }

        public static string ToHex(this BigInteger value)
        {
            return value.ToHex(false);
        }
    }
}
