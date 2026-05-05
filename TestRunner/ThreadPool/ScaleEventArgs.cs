using System;

namespace TestRunner.ThreadPool
{
    public class ScaleEventArgs : EventArgs
    {
        public int ThreadId { get; }
        public int TotalThreads { get; }
        public string Reason { get; }
        public DateTime Timestamp { get; }

        public ScaleEventArgs(int threadId, int totalThreads, string reason)
        {
            ThreadId = threadId;
            TotalThreads = totalThreads;
            Reason = reason;
            Timestamp = DateTime.Now;
        }
    }
}
