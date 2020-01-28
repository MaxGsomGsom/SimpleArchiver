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
            bufferPool.Initialize(threadPool.WorkersCount * 2 + 2, inputBlockSize);

            logger.Info($"{nameof(CompressOperationExecutor)}. Number of blocks {blocksCount}");

            outputStream.Write(BitConverter.GetBytes(inputBlockSize));
            outputStream.Write(BitConverter.GetBytes(blocksCount));

            for (int blockNumber = 0; blockNumber < blocksCount; blockNumber++)
            {
                var inputBlock = bufferPool.Take();
                inputBlock.FillFrom(inputStream, inputBlockSize);

                var outputBlock = bufferPool.Take();
                int number = blockNumber;
                threadPool.Enqueue(() => CompressBlock(inputBlock, number, outputBlock));
            }

            inputStream.Close();
            threadPool.Wait();
            blockStreamWriter.Wait(blocksCount);
            outputStream.Close();
        }

        private void CompressBlock(ReusableMemoryStream inputBlock, int number, ReusableMemoryStream outputBlock)
        {
            logger.Info($"{nameof(CompressOperationExecutor)}. Block {number}: input size {inputBlock.Length}, output size {outputBlock.Length}");

            var gzipStream = new GZipStream(outputBlock, CompressionMode.Compress);
            gzipStream.Write(inputBlock.ToSpan());
            inputBlock.Return();
            blockStreamWriter.Enqueue(outputBlock, number);
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Compress;
    }
}
