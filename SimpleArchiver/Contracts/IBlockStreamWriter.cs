using System;
using System.IO;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    /// <summary>
    /// Asynchronously writes data blocks into stream
    /// </summary>
    interface IBlockStreamWriter : IDisposable
    {
        /// <summary>
        /// Initialize writer
        /// </summary>
        /// <param name="stream">Output stream</param>
        void Initialize(Stream stream);

        /// <summary>
        /// Enqueue data block
        /// </summary>
        /// <param name="block">Input data</param>
        /// <param name="number">Sequential number of block</param>
        void Enqueue(IBuffer block, int number);

        /// <summary>
        /// Wait until all blocks up to <param name="blocksCount"/> are written to stream
        /// </summary>
        /// <param name="blocksCount">Number of blocks</param>
        /// <param name="cancel">Cancellation token</param>
        void Wait(int blocksCount, CancellationToken cancel = default);
    }
}
