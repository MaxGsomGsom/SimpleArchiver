
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
    }
}
