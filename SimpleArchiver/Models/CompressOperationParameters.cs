namespace SimpleArchiver.Models
{
    public sealed class CompressOperationParameters : OperationParameters
    {
        private const int DefaultBlockSize = 1024 * 1024 * 8;

        public CompressOperationParameters(string inputFileName, string outputFileName, int inputBlockSize = 0)
            : base(inputFileName, outputFileName)
        {
            InputBlockSize = inputBlockSize > 0 ? inputBlockSize : DefaultBlockSize;
        }

        public override ArchiverOperation Type { get; } = ArchiverOperation.Compress;

        public int InputBlockSize { get; }
    }
}
