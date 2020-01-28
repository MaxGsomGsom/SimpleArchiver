using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SimpleArchiver.Contracts;
using SimpleArchiver.Extensions;

namespace SimpleArchiver.Services
{
    public class BlockFileWriter : IBlockFileWriter
    {
        readonly object locker = new object();
        private readonly IBufferPool bufferPool;
        private FileStream stream;
        private int blocksCount, currentBlock;
        private Dictionary<int, MemoryStream> blocks = new Dictionary<int, MemoryStream>();
        private Thread writeThread;
        private bool streamClosed;

        public BlockFileWriter(IBufferPool bufferPool)
        {
            this.bufferPool = bufferPool;
        }

        public void Dispose()
        {
            Close();
            writeThread.Join();
        }

        public void SetLength(int blocksCount)
        {
            this.blocksCount = blocksCount;
        }

        public void Enqueue(MemoryStream block, int number)
        {
            lock (locker)
            {
                blocks.Add(number, block);
                Monitor.PulseAll(locker);
            }
        }

        public void Open(string fileName)
        {
            stream = File.Create(fileName);
            writeThread = new Thread(Consume) { IsBackground = true };
        }

        public void Close()
        {
            streamClosed = true;
            stream?.Close();
            Monitor.PulseAll(locker);
        }

        public void Wait(CancellationToken cancel = default)
        {
            while (currentBlock < blocksCount && !cancel.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
        }

        private void Consume()
        {
            while (currentBlock < blocksCount)
            {
                MemoryStream block;
                lock (locker)
                {
                    int nextBlock = currentBlock + 1;
                    while (!blocks.ContainsKey(nextBlock))
                    {
                        Monitor.Wait(locker);

                        if (streamClosed)
                        {
                            return;
                        }
                    }

                    block = blocks[nextBlock];
                    blocks.Remove(nextBlock);
                }

                var span = block.ToSpan();
                stream.Write(BitConverter.GetBytes(span.Length));
                stream.Write(span);
                Interlocked.Increment(ref currentBlock);
            }
        }
    }
}
