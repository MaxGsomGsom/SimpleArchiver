using System;
using System.IO;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    interface IBlockFileWriter : IDisposable
    {
        void SetLength(int blocksCount);

        void Enqueue(MemoryStream block, int number);

        void Open(string fileName);

        void Close();

        void Wait(CancellationToken cancel = default);
    }
}
