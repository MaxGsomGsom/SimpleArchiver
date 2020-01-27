namespace SimpleArchiver.Models
{
    public abstract class OperationParameters
    {
        protected OperationParameters(string inputFileName, string outputFileName)
        {
            InputFileName = inputFileName;
            OutputFileName = outputFileName;
        }

        public abstract ArchiverOperation Type { get; }

        public string InputFileName { get; }

        public string OutputFileName { get; }
    }
}
