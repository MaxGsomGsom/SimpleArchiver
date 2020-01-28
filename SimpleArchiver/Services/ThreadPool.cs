using System;
using System.Collections.Generic;
using System.Threading;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Services
{
    public class ThreadPool : IThreadPool
    {
        readonly object locker = new object();
        readonly List<Thread> workers = new List<Thread>(workerCount);
        readonly Queue<Action> taskQueue = new Queue<Action>();
        private const int workerCount = 8;
        private bool disposed;
        private int freeWorkers = workerCount;

        public ThreadPool()
        {
            for (int i = 0; i < workerCount; i++)
            {
                Thread thread = new Thread(Consume) { IsBackground = true };
                workers.Add(thread);
                thread.Start();
            }
        }

        public void Enqueue(Action task)
        {
            lock (locker)
            {
                taskQueue.Enqueue(task);
                Monitor.PulseAll(locker);
            }
        }

        public void Wait(CancellationToken cancel = default)
        {
            while (freeWorkers != workerCount && !cancel.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
        }

        private void Consume()
        {
            while (true)
            {
                Action task;
                lock (locker)
                {
                    while (taskQueue.Count == 0)
                    {
                        if (disposed)
                        {
                            return;
                        }

                        Monitor.Wait(locker);
                    }

                    task = taskQueue.Dequeue();
                }

                Interlocked.Decrement(ref freeWorkers);
                task();
                Interlocked.Increment(ref freeWorkers);
            }
        }

        public void Dispose()
        {
            disposed = true;
            Monitor.PulseAll(locker);

            foreach (var worker in workers)
            {
                worker.Join();
            }
        }
    }
}
