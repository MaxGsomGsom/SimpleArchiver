using System.IO;
using System.IO.Compression;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class DecompressOperationExecutor : IOperationExecutor
    {
        private readonly ILogger logger;
        private readonly IThreadPool threadPool;
        private readonly IBufferPool bufferPool;
        private readonly IBlockStreamWriter blockStreamWriter;

        public DecompressOperationExecutor(IThreadPool threadPool, IBufferPool bufferPool, IBlockStreamWriter blockStreamWriter, ILogger logger)
        {
            this.threadPool = threadPool;
            this.bufferPool = bufferPool;
            this.blockStreamWriter = blockStreamWriter;
            this.logger = logger;
        }

        public void Execute(OperationParameters parameters)
        {
            var inputStream = File.OpenRead(parameters.InputFileName);
            var inputStreamReader = new BinaryReader(inputStream);
            int outputBlockSize = inputStreamReader.ReadInt32();
            int blocksCount = inputStreamReader.ReadInt32();

            logger.Info($"{nameof(DecompressOperationExecutor)}. Number of blocks {blocksCount}");

            var outputStream = File.Create(parameters.OutputFileName);
            blockStreamWriter.Initialize(outputStream);
            bufferPool.Initialize(threadPool.WorkersCount * 2 + 2, outputBlockSize);


            for (int blockNumber = 0; blockNumber < blocksCount; blockNumber++)
            {
                var inputBlock = bufferPool.Take();
                int inputBlockSize = inputStreamReader.ReadInt32();
                inputBlock.FillFrom(inputStream, inputBlockSize);

                var outputBlock = bufferPool.Take();
                int number = blockNumber;
                threadPool.Enqueue(() => DecompressBlock(inputBlock, number, outputBlock, outputBlockSize));

                logger.Info($"{nameof(DecompressOperationExecutor)}. Block {number}: input size {inputBlockSize}, output size {outputBlockSize}");
            }

            inputStream.Close();
            threadPool.Wait();
            blockStreamWriter.Wait(blocksCount);
            outputStream.Close();
        }

        private void DecompressBlock(ReusableMemoryStream inputBlock, int number, ReusableMemoryStream outputBlock, int outputBlockSize)
        {
            var gzipStream = new GZipStream(inputBlock, CompressionMode.Decompress);
            outputBlock.FillFrom(gzipStream, outputBlockSize);
            inputBlock.Return();
            blockStreamWriter.Enqueue(outputBlock, number);
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Decompress;
    }
}
