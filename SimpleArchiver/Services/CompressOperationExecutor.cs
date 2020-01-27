using System;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class CompressOperationExecutor : IOperationExecutor
    {
        public void Execute(OperationParameters parameters)
        {
            Console.WriteLine(Type);
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Compress;
    }
}
