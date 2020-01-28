using System;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    interface IThreadPool : IDisposable
    {
        int WorkersCount { get; }

        void Enqueue(Action task);

        void Wait(CancellationToken cancel = default);
    }
}
