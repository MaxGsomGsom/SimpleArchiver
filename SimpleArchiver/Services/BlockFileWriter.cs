using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SimpleArchiver.Contracts;
using SimpleArchiver.Extensions;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class BlockFileWriter : IBlockFileWriter
    {
        private readonly ILogger logger;
        private readonly object locker = new object();
        private FileStream stream;
        private int blocksCount, currentBlock;
        private readonly Dictionary<int, ReusableMemoryStream> blocks = new Dictionary<int, ReusableMemoryStream>();
        private Thread writeThread;
        private bool streamClosed;

        public BlockFileWriter(ILogger logger)
        {
            this.logger = logger;
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

        public void Enqueue(ReusableMemoryStream block, int number)
        {
            lock (locker)
            {
                blocks.Add(number, block);
                logger.Info($"{nameof(BlockFileWriter)}. Added block {number} to write");
                Monitor.PulseAll(locker);
            }
        }

        public void Open(string fileName)
        {
            stream = File.Create(fileName);
            writeThread = new Thread(Consume) { IsBackground = true, Name = nameof(BlockFileWriter) };
            writeThread.Start();
        }

        public void Close()
        {
            lock (locker)
            {
                streamClosed = true;
                Monitor.PulseAll(locker);
            }

            stream?.Close();
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
            while (true)
            {
                ReusableMemoryStream block;
                lock (locker)
                {
                    while (!blocks.ContainsKey(currentBlock))
                    {
                        Monitor.Wait(locker);

                        if (streamClosed)
                        {
                            return;
                        }
                    }

                    block = blocks[currentBlock];
                    blocks.Remove(currentBlock);
                }

                var span = block.ToSpan();
                stream.Write(BitConverter.GetBytes(span.Length));
                stream.Write(span);
                logger.Info($"{nameof(BlockFileWriter)}. Block {currentBlock} is written");

                block.Return();

                Interlocked.Increment(ref currentBlock);
            }
        }
    }
}
