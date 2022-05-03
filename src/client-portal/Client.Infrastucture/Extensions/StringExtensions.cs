using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToMask(this string value, int length)
        {
            if (value == null) return "";
            if (length > value.Length) return "";
            return string.Format("{0}....{1}", value.Substring(0, length), value.Substring(value.Length - length, length));
        }
    }
}
