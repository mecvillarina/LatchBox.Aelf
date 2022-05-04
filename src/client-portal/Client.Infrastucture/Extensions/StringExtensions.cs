﻿namespace Client.Infrastructure.Extensions
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

        public static string ToMask(this string value, int length)
        {
            if (value == null) return "";
            if (length > value.Length) return "";
            return string.Format("{0}....{1}", value.Substring(0, length), value.Substring(value.Length - length, length));
        }
    }
}
