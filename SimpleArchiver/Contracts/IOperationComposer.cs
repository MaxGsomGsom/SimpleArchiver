using System.Collections.Generic;
using SimpleArchiver.Models;

namespace SimpleArchiver.Contracts
{
    public interface IOperationComposer
    {
        OperationParameters Compose(string[] args);
    }
}
