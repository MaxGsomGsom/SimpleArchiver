using System;
using System.IO;

namespace SimpleArchiver.Extensions
{
    public static class MemoryStreamExtensions
    {
        public static Span<byte> ToSpan(this MemoryStream stream)
        {
            return new Span<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        }
    }
}
