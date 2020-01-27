namespace SimpleArchiver.Models
{
    public abstract class OperationParameters
    {
        public abstract ArchiverOperation Type { get; }

        public string InputFileName { get; }
    }
}
