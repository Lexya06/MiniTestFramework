using System;

namespace TestRunner.ThreadPool
{
    public class TaskEventArgs : EventArgs
    {
        public Guid TaskId { get; }
        public int QueueLength { get; }
        public DateTime Timestamp { get; }

        public TaskEventArgs(Guid taskId, int queueLength)
        {
            TaskId = taskId;
            QueueLength = queueLength;
            Timestamp = DateTime.Now;
        }
    }
}
