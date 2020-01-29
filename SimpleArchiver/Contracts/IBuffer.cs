using System;
using System.IO;

namespace SimpleArchiver.Contracts
{
    /// <summary>
    /// Represent reusable memory buffer
    /// </summary>
    public interface IBuffer
    {
        /// <summary>
        /// Fill buffer from stream
        /// </summary>
        /// <param name="inputStream">Stream</param>
        /// <param name="size">Size of data in stream</param>
        void FillFrom(Stream inputStream, int size);

        /// <summary>
        /// Represent underlying data as span
        /// </summary>
        /// <returns></returns>
        ReadOnlySpan<byte> ToSpan();

        /// <summary>
        /// Represent underlying data as stream
        /// </summary>
        /// <returns></returns>
        Stream ToStream();

        /// <summary>
        /// Clear buffer
        /// </summary>
        void Clear();

        /// <summary>
        /// Return buffer to <see cref="IBufferPool"/>
        /// </summary>
        void Return();
    }
}
