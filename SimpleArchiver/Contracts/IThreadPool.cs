using System;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    /// <summary>
    /// Represent worker threads pool. Number of threads equals to number of CPU cores
    /// </summary>
    interface IThreadPool : IDisposable
    {
        int WorkersCount { get; }

        /// <summary>
        /// Enqueue task to execute on worker thread
        /// </summary>
        /// <param name="task"></param>
        void Enqueue(Action task);

        /// <summary>
        /// Wait until all queued tasks are executed
        /// </summary>
        /// <param name="cancel"></param>
        void Wait(CancellationToken cancel = default);
    }
}
