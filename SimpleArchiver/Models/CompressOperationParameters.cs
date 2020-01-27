namespace SimpleArchiver.Models
{
    public sealed class CompressOperationParameters : OperationParameters
    {
        public override ArchiverOperation Type { get; }

        public int InputBlockSize { get; }

        public string OutputFileName { get; }
    }
}
