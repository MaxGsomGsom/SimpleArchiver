using System;
using System.IO;
using System.Threading;
using SimpleArchiver.Models;

namespace SimpleArchiver.Contracts
{
    interface IBlockStreamWriter : IDisposable
    {
        void Initialize(Stream stream);

        void Enqueue(ReusableMemoryStream block, int number);

        void Wait(int blocksCount, CancellationToken cancel = default);
    }
}
