using System;
using System.IO;
using SimpleArchiver.Contracts;
using SimpleArchiver.Models;

namespace SimpleArchiver.Services
{
    internal sealed class OperationComposer : IOperationComposer
    {
        public OperationParameters Compose(string[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("Invalid arguments. Required: compress/decompress inputFileName outputFileName");
            }

            Enum.TryParse(args[0], true, out ArchiverOperation operation);

            if (operation == ArchiverOperation.None)
            {
                throw new ArgumentException($"Invalid operation: {args[0]}. Available: compress, decompress");
            }

            var inputFileName = args[1];
            if (!File.Exists(inputFileName))
            {
                throw new ArgumentException($"Input file {inputFileName} doesn't exist");
            }


            var outputFileName = args[2];
            if (File.Exists(outputFileName))
            {
                throw new ArgumentException($"Output file {outputFileName} already exists");
            }

            int inputBlockSize = 0;
            if (args.Length >= 4)
            {
                int.TryParse(args[3], out inputBlockSize);
            }
            return new OperationParameters(operation, inputFileName, outputFileName, inputBlockSize);
        }
    }
}
