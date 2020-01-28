using System;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Services
{
    internal sealed class DebugConsoleLogger : ILogger
    {
        public void Info(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
