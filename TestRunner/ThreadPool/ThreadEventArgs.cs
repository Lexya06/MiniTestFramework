using System;

namespace TestRunner.ThreadPool
{
    public class ThreadEventArgs : EventArgs
    {
        public int ThreadId { get; }
        public int TotalThreads { get; }
        public DateTime Timestamp { get; }

        public ThreadEventArgs(int threadId, int totalThreads)
        {
            ThreadId = threadId;
            TotalThreads = totalThreads;
            Timestamp = DateTime.Now;
        }
    }
}
