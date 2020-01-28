using System;
using System.Threading;
using SimpleArchiver.Models;

namespace SimpleArchiver.Contracts
{
    public interface IBufferPool : IDisposable
    {
        void Initialize(int count, int bufferSize);

        ReusableMemoryStream Take(CancellationToken cancel = default);

        void Return(ReusableMemoryStream buffer);
    }
}
