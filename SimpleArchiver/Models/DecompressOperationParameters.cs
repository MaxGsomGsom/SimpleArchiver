namespace SimpleArchiver.Models
{
    public sealed class DecompressOperationParameters : OperationParameters
    {
        public DecompressOperationParameters(string inputFileName, string outputFileName)
            : base(inputFileName, outputFileName)
        {
        }

        public override ArchiverOperation Type { get; } = ArchiverOperation.Decompress;
    }
}
