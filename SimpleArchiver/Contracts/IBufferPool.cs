using System;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    /// <summary>
    /// Represent pool of reusable memory streams
    /// </summary>
    public interface IBufferPool : IDisposable
    {
        /// <summary>
        /// Initialize pool
        /// </summary>
        /// <param name="count">Number of buffers</param>
        /// <param name="bufferSize">Initial buffer size</param>
        void Initialize(int count, int bufferSize);

        /// <summary>
        /// Take free buffer from pool
        /// </summary>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>Stream</returns>
        IBuffer Take(CancellationToken cancel = default);

        /// <summary>
        /// Cleanup buffer and return to pool
        /// </summary>
        /// <param name="buffer"></param>
        void Return(IBuffer buffer);
    }
}
