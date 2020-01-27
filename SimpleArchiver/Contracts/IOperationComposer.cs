using SimpleArchiver.Models;

namespace SimpleArchiver.Contracts
{
    public interface IOperationComposer
    {
        OperationParameters Compose(string[] args);
    }
}
