using System;
using System.Collections.Generic;
using System.Threading;
using SimpleArchiver.Contracts;

namespace SimpleArchiver.Services
{
    public class ThreadPool : IThreadPool
    {
        public int WorkersCount { get; }
        readonly object locker = new object();
        private readonly List<Thread> workers;
        readonly Queue<Action> taskQueue = new Queue<Action>();
        private bool disposed;
        private int freeWorkers;

        public ThreadPool()
        {
            WorkersCount = Environment.ProcessorCount;
            freeWorkers = WorkersCount;
            workers = new List<Thread>(WorkersCount);

            for (int i = 0; i < WorkersCount; i++)
            {
                Thread thread = new Thread(Consume) { IsBackground = true, Name = $"{nameof(ThreadPool)} {i}"};
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
            while (freeWorkers != WorkersCount && !cancel.IsCancellationRequested)
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
            lock (locker)
            {
                disposed = true;
                Monitor.PulseAll(locker);
            }

            foreach (var worker in workers)
            {
                worker.Join();
            }
        }
    }
}
