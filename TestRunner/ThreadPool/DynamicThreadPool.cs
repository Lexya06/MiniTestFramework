using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRunner.Runner;

using System;
using System.Collections.Generic;
using System.Threading;

namespace TestRunner.ThreadPool
{
    public class DynamicThreadPool : IDisposable
    {
        private readonly int _minThreads;
        private readonly int _maxThreads;
        private readonly TimeSpan _idleTimeout;
        private readonly TimeSpan _hangTimeout;
        private readonly TimeSpan _taskTimeout;
        private readonly Logger _logger;

        private readonly Queue<(Guid id, Action task)> _taskQueue = new();
        private readonly Dictionary<Guid, DateTime> _waitingTasks = new();
        private readonly Dictionary<Thread, DateTime> _executingThreads = new();
        private readonly List<Thread> _workers = new();

        private readonly object _lock = new();
        private bool _isDisposed;
        private Thread _supervisorThread;

        public DynamicThreadPool(int minThreads, int maxThreads, TimeSpan idleTimeout, TimeSpan hangTimeout, TimeSpan taskTimeout, Logger logger)
        {
            _minThreads = minThreads;
            _maxThreads = maxThreads;
            _idleTimeout = idleTimeout;
            _hangTimeout = hangTimeout;
            _taskTimeout = taskTimeout;
            _logger = logger;

            for (int i = 0; i < _minThreads; i++)
                StartNewThread();

            _supervisorThread = new Thread(SupervisorLoop)
            {
                IsBackground = true,
                Name = "Supervisor"
            };
            _supervisorThread.Start();
        }

        private void StartNewThread()
        {
            var thread = new Thread(WorkerLoop)
            {
                IsBackground = true
            };

            lock (_lock)
            {
                _workers.Add(thread);
            }

            thread.Start();
            lock (_lock)
            {
                _logger.LogEvent($"Scale UP: поток {thread.ManagedThreadId}, всего потоков: {_workers.Count}, ids: [{string.Join(", ", _workers.ConvertAll(t => t.ManagedThreadId))}]");
            }
        }

        public void EnqueueTask(Action task)
        {
            lock (_lock)
            {
                if (_isDisposed) throw new ObjectDisposedException(nameof(DynamicThreadPool));

                var id = Guid.NewGuid();
                _taskQueue.Enqueue((id, task));
                _waitingTasks[id] = DateTime.Now;

                _logger.LogEvent($"Task queued. Count: {_taskQueue.Count}");

                Monitor.PulseAll(_lock);

                if (_taskQueue.Count > _workers.Count && _workers.Count < _maxThreads)
                {
                    StartNewThread();
                }
            }
        }

        private void WorkerLoop()
        {
            while (true)
            {
                (Guid id, Action task) work;

                lock (_lock)
                {
                    while (_taskQueue.Count == 0)
                    {
                        if (_isDisposed) return;

                        bool signaled = Monitor.Wait(_lock, _idleTimeout);

                        if (!signaled && _workers.Count > _minThreads)
                        {
                            _workers.Remove(Thread.CurrentThread);
                            _logger.LogEvent($"Scale DOWN: поток {Thread.CurrentThread.ManagedThreadId}, всего потоков: {_workers.Count}, ids: [{string.Join(", ", _workers.ConvertAll(t => t.ManagedThreadId))}]");
                            return;
                        }
                    }

                    work = _taskQueue.Dequeue();
                    _waitingTasks.Remove(work.id);
                    _executingThreads[Thread.CurrentThread] = DateTime.Now;
                }

                try
                {
                    work.task();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Task error: {ex.Message}");
                }
                finally
                {
                    lock (_lock)
                    {
                        _executingThreads.Remove(Thread.CurrentThread);

                        if (_workers.Count < _minThreads)
                            StartNewThread();
                    }
                }
            }
        }

        private void SupervisorLoop()
        {
            while (!_isDisposed)
            {
                

                lock (_lock)
                {
                    var now = DateTime.Now;

                    // hung threads
                    foreach (var kvp in new List<KeyValuePair<Thread, DateTime>>(_executingThreads))
                    {
                        if ((now - kvp.Value) > _hangTimeout)
                        {
                            _logger.LogError($"Thread {kvp.Key.ManagedThreadId} is hung. Всего потоков: {_workers.Count}, ids: [{string.Join(", ", _workers.ConvertAll(t => t.ManagedThreadId))}]");

                            _executingThreads.Remove(kvp.Key);

                            if (_workers.Count < _maxThreads)
                                StartNewThread();
                        }
                    }

                    // long waiting tasks
                    foreach (var kvp in new List<KeyValuePair<Guid, DateTime>>(_waitingTasks))
                    {
                        if ((now - kvp.Value) > _taskTimeout)
                        {
                            _logger.LogEvent("Task waited too long -> scaling up");

                            if (_workers.Count < _maxThreads)
                                StartNewThread();
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _isDisposed = true;
                Monitor.PulseAll(_lock);
            }
        }
    }
}



