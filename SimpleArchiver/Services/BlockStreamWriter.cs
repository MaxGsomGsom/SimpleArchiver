using System.Collections.Generic;
using System.IO;
using System.Threading;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Services
{
    internal sealed class BlockStreamWriter : IBlockStreamWriter
    {
        private readonly ILogger logger;
        private readonly object locker = new object();
        private Stream stream;
        private int currentBlock;
        private readonly Dictionary<int, IBuffer> blocks = new Dictionary<int, IBuffer>();
        private Thread writeThread;
        private bool disposed;

        public BlockStreamWriter(ILogger logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            lock (locker)
            {
                disposed = true;
                Monitor.PulseAll(locker);
            }

            writeThread?.Join();
        }

        public void Enqueue(IBuffer block, int number)
        {
            lock (locker)
            {
                blocks.Add(number, block);
                logger.Debug($"{nameof(BlockStreamWriter)}. Added block {number} to write");
                Monitor.PulseAll(locker);
            }
        }

        public void Initialize(Stream stream)
        {
            this.stream = stream;
            writeThread = new Thread(Consume) { IsBackground = true, Name = nameof(BlockStreamWriter) };
            writeThread.Start();
        }

        public void Wait(int blocksCount, CancellationToken cancel = default)
        {
            while (currentBlock < blocksCount && !cancel.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
        }

        private void Consume()
        {
            while (true)
            {
                IBuffer block;
                lock (locker)
                {
                    while (!blocks.ContainsKey(currentBlock))
                    {
                        Monitor.Wait(locker);

                        if (disposed)
                        {
                            return;
                        }
                    }

                    block = blocks[currentBlock];
                    blocks.Remove(currentBlock);
                }


                stream.Write(block.ToSpan());

                logger.Debug($"{nameof(BlockStreamWriter)}. Block {currentBlock} is written");

                block.Return();

                Interlocked.Increment(ref currentBlock);
            }
        }
    }
}
