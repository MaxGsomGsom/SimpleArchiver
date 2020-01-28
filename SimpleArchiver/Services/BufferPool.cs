using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Services
{
    public class BufferPool : IBufferPool
    {
        private int totalCount;
        private readonly SemaphoreSlim semaphore;
        private readonly Dictionary<MemoryStream, bool> buffers = new Dictionary<MemoryStream, bool>();

        public BufferPool()
        {
            semaphore = new SemaphoreSlim(totalCount);
        }

        public void Initialize(int count, int initialBufferSize)
        {
            for (int i = 0; i < totalCount; i++)
            {
                buffers.Add(new MemoryStream(initialBufferSize), true);
            }

            totalCount = count;
        }

        public MemoryStream Take(CancellationToken cancel = default)
        {
            semaphore.Wait(cancel);

            var freeBuffer = buffers.First(e => e.Value).Key;
            buffers[freeBuffer] = false;
            return freeBuffer;
        }

        public void Return(MemoryStream buffer)
        {
            if (!buffers.ContainsKey(buffer))
            {
                return;
            }

            buffer.SetLength(0);
            buffers[buffer] = true;

            semaphore.Release();
        }

        public void Dispose()
        {
            semaphore.Dispose();
            foreach (var buffer in buffers)
            {
                buffer.Key.Dispose();
            }
        }
    }
}
