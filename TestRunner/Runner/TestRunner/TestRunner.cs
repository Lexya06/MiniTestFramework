using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestRunner.Models;
using TestRunner.ThreadPool;

namespace TestRunner.Runner
{
    public class TestRunner : Runner
    {
        
        
        
        private async Task RunTestAsync(ClassInfo classInfo, object? instance, TestInfo testInfo, TestExecutor executor, SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync();
           
            try
            {
                TestResult testRes = await Task.Run(() => executor.GetTestResult(instance, testInfo, cancellationToken));
                classInfo.TestResults.Add(testRes);
            }
            finally
            {
                semaphore.Release();
            }

        }

        private void RunTestSync(ClassInfo classInfo, object? instance, TestInfo testInfo, TestExecutor executor, CancellationToken cancellationToken)
        {
            TestResult testRes = executor.GetTestResult(instance, testInfo, cancellationToken);
            classInfo.TestResults.Add(testRes);
        }

        public async Task<long> RunTestsAsync(List<ClassInfo> classInfos, int threads, TestExecutor executor)
        {
            if (threads <= 0)
            {
                throw new ArgumentException("Number of threads to work is less zero");
            }
           
            SemaphoreSlim semaphore = new SemaphoreSlim(threads, threads);

            Stopwatch stopwatch = Stopwatch.StartNew();
            var classGroups = classInfos.GroupBy(g => g.getPriority());
            foreach (var classGroup in classGroups)
            {
                foreach (var classInfo in classGroup)
                {
                    
                    object? instance = Activator.CreateInstance(classInfo.ClassType);
                    if (classInfo.TestResults.Count > 0)
                    {
                        classInfo.TestResults.Clear();
                    }
                    var testGroups = classInfo.TestInfos.GroupBy(g => g.getPriority()).OrderBy(g => g.Key);
                    foreach(var testGroup in testGroups)
                    {
                        List<Task> groupTasks = new List<Task>();
                        foreach (var testInfo in testGroup)
                        {
                            
                            CancellationToken cancellationToken = CancellationToken.None;
                            CancellationTokenSource? cancellationTokenSource = null;
                            if (testInfo.getTimeout().HasValue)
                            {
                                cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(testInfo.getTimeout()!.Value)); 
                                cancellationToken = cancellationTokenSource.Token;
                            }
                            if (classInfo.getShared())
                                instance = Activator.CreateInstance(classInfo.ClassType);
                            groupTasks.Add(RunTestAsync(classInfo, instance, testInfo, executor, semaphore, cancellationToken));
                        }

                        await Task.WhenAll(groupTasks);
                    }
                }
               
            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public long RunTestsSync(List<ClassInfo> classInfos, TestExecutor executor)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();
            var classGroups = classInfos.GroupBy(g => g.getPriority());
            foreach (var classGroup in classGroups)
            {
                foreach (var classInfo in classGroup)
                {

                    object? instance = Activator.CreateInstance(classInfo.ClassType);
                    if (classInfo.TestResults.Count > 0)
                    {
                        classInfo.TestResults.Clear();
                    }
                    var testGroups = classInfo.TestInfos.GroupBy(g => g.getPriority()).OrderBy(g => g.Key);
                    foreach (var testGroup in testGroups)
                    {
                        List<Task> groupTasks = new List<Task>();
                        foreach (var testInfo in testGroup)
                        {
                            CancellationToken cancellationToken = CancellationToken.None;
                            CancellationTokenSource? cancellationTokenSource = null;
                            if (testInfo.getTimeout().HasValue)
                            {
                                cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(testInfo.getTimeout()!.Value));
                                cancellationToken = cancellationTokenSource.Token;
                            }
                            if (classInfo.getShared())
                                instance = Activator.CreateInstance(classInfo.ClassType);
                            RunTestSync(classInfo, instance, testInfo, executor, cancellationToken);
                        }
                       
                    }
                }
               

            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;

        }

        public long RunTests(List<ClassInfo> classInfos, TestExecutor executor)
        {
            return RunTestsSync(classInfos, executor);
        }
    }
}
