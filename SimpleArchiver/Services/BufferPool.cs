using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class BufferPool : IBufferPool
    {
        private readonly ILogger logger;
        private int totalCount;
        private SemaphoreSlim semaphore;
        private readonly Dictionary<ReusableMemoryStream, bool> buffers = new Dictionary<ReusableMemoryStream, bool>();

        public BufferPool(ILogger logger)
        {
            this.logger = logger;
            semaphore = new SemaphoreSlim(totalCount);
        }

        public void Initialize(int count, int initialBufferSize)
        {
            for (int i = 0; i < count; i++)
            {
                buffers.Add(new ReusableMemoryStream(initialBufferSize, this), true);
            }

            totalCount = count;
            semaphore?.Dispose();
            semaphore = new SemaphoreSlim(count);
        }

        public ReusableMemoryStream Take(CancellationToken cancel = default)
        {
            semaphore.Wait(cancel);
            logger.Info($"{nameof(BufferPool)}. Buffer taken. Free buffers {semaphore.CurrentCount}");

            var freeBuffer = buffers.First(e => e.Value).Key;
            buffers[freeBuffer] = false;
            return freeBuffer;
        }

        public void Return(ReusableMemoryStream buffer)
        {
            if (!buffers.ContainsKey(buffer))
            {
                return;
            }

            buffer.SetLength(0);
            buffers[buffer] = true;

            semaphore.Release();

            logger.Info($"{nameof(BufferPool)}. Buffer returned. Free buffers {semaphore.CurrentCount}");
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
