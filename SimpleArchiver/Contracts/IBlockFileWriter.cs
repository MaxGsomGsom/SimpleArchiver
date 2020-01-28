using System;
using System.Threading;
using SimpleArchiver.Models;

namespace SimpleArchiver.Contracts
{
    interface IBlockFileWriter : IDisposable
    {
        void SetLength(int blocksCount);

        void Enqueue(ReusableMemoryStream block, int number);

        void Open(string fileName);

        void Close();

        void Wait(CancellationToken cancel = default);
    }
}
