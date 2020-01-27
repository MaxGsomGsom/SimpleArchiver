using System;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    public class CompressOperationExecutor : IOperationExecutor
    {
        public void Execute(OperationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Compress;
    }
}
