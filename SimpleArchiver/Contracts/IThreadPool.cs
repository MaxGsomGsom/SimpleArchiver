using System;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    interface IThreadPool : IDisposable
    {
        void Enqueue(Action task);

        void Wait(CancellationToken cancel = default);
    }
}
