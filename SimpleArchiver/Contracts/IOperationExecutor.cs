using SimpleArchiver.Models;

namespace SimpleArchiver.Contracts
{
    interface IOperationExecutor
    {
        void Execute(OperationParameters parameters);

        ArchiverOperation Type { get; }
    }
}
