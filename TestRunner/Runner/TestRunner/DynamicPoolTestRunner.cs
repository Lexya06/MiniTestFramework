using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRunner.Models;
using TestRunner.ThreadPool;

namespace TestRunner.Runner
{
    public class DynamicPoolTestRunner : Runner
    {
        private readonly DynamicThreadPool _threadPool;

        public DynamicPoolTestRunner(DynamicThreadPool threadPool)
        {
            _threadPool = threadPool;
        }

        public long RunTests(List<ClassInfo> classInfos, TestExecutor executor)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            var classGroups = classInfos.GroupBy(g => g.getPriority());
            foreach (var classGroup in classGroups)
            {
                foreach (var classInfo in classGroup)
                {
                    object? instance = Activator.CreateInstance(classInfo.ClassType);
                    if (classInfo.TestResults.Count > 0) classInfo.TestResults.Clear();

                    var testGroups = classInfo.TestInfos.GroupBy(g => g.getPriority()).OrderBy(g => g.Key);
                    foreach (var testGroup in testGroups)
                    {
                        int totalTests = testGroup.Count();
                        // Semaphore с начальным счётчиком 0
                        using (var semaphore = new Semaphore(0, totalTests))
                        {

                            foreach (var testInfo in testGroup)
                            {
                                CancellationTokenSource? cts = null;
                                if (testInfo.getTimeout().HasValue)
                                {
                                    cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(testInfo.getTimeout()!.Value));
                                }

                                if (classInfo.getShared())
                                    instance = Activator.CreateInstance(classInfo.ClassType);

                                var capturedInstance = instance;
                                var capturedTestInfo = testInfo;
                                var capturedToken = cts?.Token ?? CancellationToken.None;


                                _threadPool.EnqueueTask(() =>
                                {
                                    try
                                    {
                                        TestResult testRes = executor.GetTestResult(capturedInstance, capturedTestInfo, capturedToken);
                                        classInfo.TestResults.Add(testRes);

                                    }
                                    finally
                                    {
                                        cts?.Dispose();
                                        semaphore.Release();
                                    }
                                });
                            }
                           
                            for (int i = 0; i < totalTests; i++)
                            {
                                semaphore.WaitOne();
                            }
                        }

                    }
                }
            }

                
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
