using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Extensions
{
    public static class StreamExtension
    {
        public static MemoryStream ToStream(this string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
