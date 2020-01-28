namespace SimpleArchiver.Models
{
    public sealed class OperationParameters
    {
        private const int DefaultBlockSize = 1024 * 1024 * 1;

        public OperationParameters(ArchiverOperation type, string inputFileName, string outputFileName, int inputBlockSize = 0)
        {
            Type = type;
            InputFileName = inputFileName;
            OutputFileName = outputFileName;
            InputBlockSize = inputBlockSize > 0 ? inputBlockSize : DefaultBlockSize;
        }

        public ArchiverOperation Type { get; }

        public string InputFileName { get; }

        public string OutputFileName { get; }

        public int InputBlockSize { get; }
    }
}
