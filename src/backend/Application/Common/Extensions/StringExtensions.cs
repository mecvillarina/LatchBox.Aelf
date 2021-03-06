using AElf.Types;
using AElf;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;

namespace Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToAmountDisplay(this decimal amount, int decimals)
        {
            return amount.ToString($"0.{new string('#', decimals)}");
        }

        public static string ToAmountDisplay(this long amount, int decimals)
        {
            return amount.ToAmount(decimals).ToAmountDisplay(decimals);
        }

        public static string ToStringAddress(this Address address)
        {
            var newAdd = new AElf.Types.Address() { Value = address.Value };
            return newAdd.ToBase58();
        }

        public static string ToChainIdBase58(this int chainId)
        {
            return ChainHelper.ConvertChainIdToBase58(chainId);
        }

        public static string RemoveTrailingZeros(this string num)
        {
            decimal numVal = Convert.ToDecimal(num);
            numVal = numVal / 1.000000000000000000000000000000000m;
            return numVal.ToString();
        }

        public static string ToMask(this string value, int length)
        {
            if (value == null) return "";
            if (length > value.Length) return "";
            return string.Format("{0}....{1}", value.Substring(0, length), value.Substring(value.Length - length, length));
        }
    }
}