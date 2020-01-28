using System;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Services
{
    internal sealed class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }
}
