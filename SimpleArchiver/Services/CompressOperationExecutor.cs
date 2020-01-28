using System;
using System.IO;
using System.IO.Compression;
using SimpleArchiver.Contracts;
using SimpleArchiver.Extensions;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class CompressOperationExecutor : IOperationExecutor
    {
        private readonly IThreadPool threadPool;
        private readonly IBufferPool bufferPool;
        private readonly IBlockFileWriter blockFileWriter;

        public CompressOperationExecutor(
            IThreadPool threadPool,
            IBufferPool bufferPool,
            IBlockFileWriter blockFileWriter)
        {
            this.threadPool = threadPool;
            this.bufferPool = bufferPool;
            this.blockFileWriter = blockFileWriter;
        }

        public void Execute(OperationParameters parameters)
        {
            var inputStream = File.OpenRead(parameters.InputFileName);
            blockFileWriter.Open(parameters.OutputFileName);
            var blockSize = parameters.InputBlockSize;
            var blocksCount = inputStream.Length / blockSize +
                              inputStream.Length % blockSize == 0 ? 0 : 1;
            bufferPool.Initialize(8, blockSize);


            for (int blockNumber = 0; blockNumber < blocksCount; blockNumber++)
            {
                var inputBlock = bufferPool.Take();
                inputStream.Read(inputBlock.GetBuffer(), 0, blockSize);

                var outputBlock = bufferPool.Take();
                int number = blockNumber;
                threadPool.Enqueue(() => CompressBlock(inputBlock, number, outputBlock));
            }

            inputStream.Close();
            threadPool.Wait();
            blockFileWriter.Wait();
            blockFileWriter.Close();
        }

        private void CompressBlock(MemoryStream inputBlock, int number, MemoryStream outputBlock)
        {
            var gzipStream = new GZipStream(outputBlock, CompressionMode.Compress);
            gzipStream.Write(inputBlock.ToSpan());
            blockFileWriter.Enqueue(outputBlock, number);
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Compress;
    }
}
