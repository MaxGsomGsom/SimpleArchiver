using System;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class DecompressOperationExecutor : IOperationExecutor
    {
        public void Execute(OperationParameters parameters)
        {
            Console.WriteLine(Type);
        }

        public ArchiverOperation Type { get; } = ArchiverOperation.Decompress;
    }
}
