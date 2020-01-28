using System;
using System.IO;
using System.Threading;

namespace SimpleArchiver.Contracts
{
    public interface IBufferPool : IDisposable
    {
        void Initialize(int count, int bufferSize);

        MemoryStream Take(CancellationToken cancel = default);

        void Return(MemoryStream buffer);
    }
}
