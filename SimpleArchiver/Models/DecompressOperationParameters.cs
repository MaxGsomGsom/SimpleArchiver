namespace SimpleArchiver.Models
{
    public sealed class DecompressOperationParameters : OperationParameters
    {
        public override ArchiverOperation Type { get; }

        public string OutputFileName { get; }
    }
}
