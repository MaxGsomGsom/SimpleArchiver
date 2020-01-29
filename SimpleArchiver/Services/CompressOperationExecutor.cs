using System;
using System.IO;
using System.IO.Compression;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class CompressOperationExecutor : IOperationExecutor
    {
        private readonly ILogger logger;
        private readonly IThreadPool threadPool;
        private readonly IBufferPool bufferPool;
        private readonly IBlockStreamWriter blockStreamWriter;

        public CompressOperationExecutor(
            IThreadPool threadPool,
            IBufferPool bufferPool,
            IBlockStreamWriter blockStreamWriter, ILogger logger)
        {
            this.threadPool = threadPool;
            this.bufferPool = bufferPool;
            this.blockStreamWriter = blockStreamWriter;
            this.logger = logger;
        }

        public void Execute(OperationParameters parameters)
        {
            var inputStream = File.OpenRead(parameters.InputFileName);
            var outputStream = File.Create(parameters.OutputFileName);
            blockStreamWriter.Initialize(outputStream);

            int inputBlockSize = parameters.InputBlockSize;
            int blocksCount = (int)(inputStream.Length / inputBlockSize) + (inputStream.Length % inputBlockSize == 0 ? 0 : 1);
            int lastInputBlockSize = (int)(inputStream.Length - inputBlockSize * (blocksCount - 1));
            bufferPool.Initialize(Math.Min(threadPool.WorkersCount, blocksCount) * 3, inputBlockSize);

            logger.Debug($"{nameof(CompressOperationExecutor)}. Number of blocks {blocksCount}");

            outputStream.Write(BitConverter.GetBytes(inputStream.Length));
            outputStream.Write(BitConverter.GetBytes(inputBlockSize));

            for (int blockNumber = 0; blockNumber < blocksCount; blockNumber++)
            {
                var inputBlock = bufferPool.Take();
                int currentInputBlockSize = blockNumber == blocksCount - 1 ? lastInputBlockSize : inputBlockSize;
                inputBlock.FillFrom(inputStream, currentInputBlockSize);

                var outputBlock = bufferPool.Take();
                int number = blockNumber;
                threadPool.Enqueue(() => CompressBlock(inputBlock, number, outputBlock));
            }

            inputStream.Close();
            threadPool.Wait();
            blockStreamWriter.Wait(blocksCount);
            outputStream.Close();
        }

        private void CompressBlock(IBuffer inputBlock, int number, IBuffer outputBlock)
        {
            const int prefix = sizeof(int);
            var stream = outputBlock.ToStream();
            stream.Position = prefix;
            using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                gzipStream.Write(inputBlock.ToSpan());
            }

            stream.Position = 0;
            stream.Write(BitConverter.GetBytes((int)stream.Length - prefix));

            logger.Debug($"{nameof(CompressOperationExecutor)}. Block {number}: input size {stream.Length}, output size {stream.Length - 4}");

            inputBlock.Return();
            blockStreamWriter.Enqueue(outputBlock, number);
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Compress;
    }
}
