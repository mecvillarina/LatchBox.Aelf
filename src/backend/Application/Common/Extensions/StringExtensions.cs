using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToEmptyIfNull(this string s)
        {
            return s ?? string.Empty;
        }

        public static string ToNormalize(this string s)
        {
            s ??= string.Empty;
            s = s.Replace(" ", string.Empty).ToUpper();
            return s;
        }

        public static string ToSingleSpacing(this string str)
        {
            var options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);
            str ??= string.Empty;
            str = regex.Replace(str, " ");

            return str;
        }

        public static string ToHexString(this string str)
        {
            str ??= string.Empty;
            var ca = str.Reverse().ToArray();
            var ba = Encoding.Default.GetBytes(ca);

            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static string FromHexStringToString(this string hexStr)
        {
            hexStr = hexStr.Replace("-", "");
            byte[] raw = new byte[hexStr.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hexStr.Substring(i * 2, 2), 16);
            }

            return Encoding.Default.GetString(raw);
        }
    }
}