
using System;
using System.IO;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Models
{
    public sealed class ReusableMemoryStream : MemoryStream
    {
        private readonly IBufferPool pool;

        public ReusableMemoryStream(int capacity, IBufferPool pool)
            : base(capacity)
        {
            this.pool = pool;
        }

        public void Return()
        {
            pool.Return(this);
        }

        public Span<byte> ToSpan()
        {
            return new Span<byte>(GetBuffer(), 0, (int)Length);
        }

        public void FillFrom(Stream stream, int size)
        {
            SetLength(size);
            stream.Read(GetBuffer(), 0, size);
        }
    }
}
