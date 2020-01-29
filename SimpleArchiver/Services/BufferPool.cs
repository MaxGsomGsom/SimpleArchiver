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
        private int currentCount;
        private readonly object locker = new object();
        private readonly Dictionary<IBuffer, bool> buffers = new Dictionary<IBuffer, bool>();

        public BufferPool(ILogger logger)
        {
            this.logger = logger;
        }

        public void Initialize(int count, int initialBufferSize)
        {
            for (int i = 0; i < count; i++)
            {
                buffers.Add(new BufferMemoryStream(initialBufferSize, this), true);
            }

            currentCount = count;
        }

        public IBuffer Take(CancellationToken cancel = default)
        {
            IBuffer freeBuffer;
            lock (locker)
            {
                while (currentCount == 0)
                {
                    Monitor.Wait(locker);
                }

                freeBuffer = buffers.First(e => e.Value).Key;
                buffers[freeBuffer] = false;
                currentCount--;
            }

            logger.Debug($"{nameof(BufferPool)}. Buffer taken. Free buffers {currentCount}");

            return freeBuffer;
        }

        public void Return(IBuffer buffer)
        {
            if (!buffers.ContainsKey(buffer))
            {
                return;
            }

            buffer.Clear();

            lock (locker)
            {
                buffers[buffer] = true;
                currentCount++;
                Monitor.PulseAll(locker);
            }

            logger.Debug($"{nameof(BufferPool)}. Buffer returned. Free buffers {currentCount}");
        }

        public void Dispose()
        {
            foreach (var buffer in buffers)
            {
                (buffer.Key as BufferMemoryStream)?.Dispose();
            }
        }
    }
}
