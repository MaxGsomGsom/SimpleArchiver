
using System;
using System.IO;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Models
{
    internal sealed class BufferMemoryStream : MemoryStream, IBuffer
    {
        private readonly IBufferPool pool;

        public BufferMemoryStream(int capacity, IBufferPool pool)
            : base(capacity)
        {
            this.pool = pool;
        }

        public Stream ToStream()
        {
            return this;
        }

        public void Clear()
        {
            SetLength(0);
        }

        public void Return()
        {
            pool.Return(this);
        }

        public ReadOnlySpan<byte> ToSpan()
        {
            return new ReadOnlySpan<byte>(GetBuffer(), 0, (int)Length);
        }

        public void FillFrom(Stream inputStream, int size)
        {
            SetLength(size);
            inputStream.Read(GetBuffer(), 0, size);
        }
    }
}
