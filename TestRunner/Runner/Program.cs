using System.Diagnostics;
using System.Reflection;
using MiniTestFramework.Attributes.ClassAttributes;
using MiniTestFramework.Attributes.MethodAttributes;
using MiniTestFramework.Exceptions;
using TestRunner.Models;
using TestRunner.ThreadPool;
using TestRunner.Utils;

namespace TestRunner.Runner
{

    class Program
    {

        public static int GetMaxParallelThreads()
        {
            int minThreads = 1;
            int threads = 0;
            bool flag = false;
            do
            {
                Console.WriteLine("\nEnter max number of threads to parallel:");
                if (int.TryParse(Console.ReadLine(), out threads) && threads >= minThreads)
                {
                    flag = true;
                }
                else
                    Console.WriteLine("Please enter a positive integer.");
            } while (!flag);
            return threads;
        }

        public static long RunXTimes(List<ClassInfo> classes, LoadSimulator simulator, TestLogger logger, int times)
        {
            long time;
            logger.LogTestsInfo($"Начало {times} прогонов");
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 1; i <= times; i++)
            {
                time = simulator.RunUnevenLoadScenario(classes);

                // Информация о запуске тестов в самом logTestsInfo
                logger.LogTestsInfo(classes, $"Запуск  #{i}. Затрачено: {time} ms");
            }

            logger.LogTestsInfo($"Конец {times} прогонов");
            stopwatch.Stop();
            logger.LogTestsInfo($"Затраченное время = {stopwatch.ElapsedMilliseconds}");
            return stopwatch.ElapsedMilliseconds;

        }

        public static void subscribePoolOnEvents(DynamicThreadPool pool)
        {
            pool.ThreadStarted += (s, e) =>
                    Console.WriteLine($"[EVENT] ThreadStarted: ThreadId={e.ThreadId}, TotalThreads={e.TotalThreads}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.ThreadStopped += (s, e) =>
                Console.WriteLine($"[EVENT] ThreadStopped: ThreadId={e.ThreadId}, TotalThreads={e.TotalThreads}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.ScaleUp += (s, e) =>
                Console.WriteLine($"[EVENT] ScaleUp: ThreadId={e.ThreadId}, TotalThreads={e.TotalThreads}, Reason={e.Reason}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.ScaleDown += (s, e) =>
                Console.WriteLine($"[EVENT] ScaleDown: ThreadId={e.ThreadId}, TotalThreads={e.TotalThreads}, Reason={e.Reason}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.ThreadHung += (s, e) =>
                Console.WriteLine($"[EVENT] ThreadHung: ThreadId={e.ThreadId}, TotalThreads={e.TotalThreads}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.TaskQueuedEvent += (s, e) =>
                Console.WriteLine($"[EVENT] TaskQueued: TaskId={e.TaskId}, QueueLength={e.QueueLength}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.TaskExecuting += (s, e) =>
                Console.WriteLine($"[EVENT] TaskExecuting: TaskId={e.TaskId}, QueueLength={e.QueueLength}, Time={e.Timestamp:HH:mm:ss.fff}");
            pool.TaskCompleted += (s, e) =>
                Console.WriteLine($"[EVENT] TaskCompleted: TaskId={e.TaskId}, QueueLength={e.QueueLength}, Time={e.Timestamp:HH:mm:ss.fff}");
        }

        static void Main()
        {
            TestLogger testLogger = new TestLogger();
            Logger logger = new Logger();
            TestExecutor executor = new TestExecutor();
            TestFinder finder = new TestFinder();

            int maxThreads = GetMaxParallelThreads();

            List<ClassInfo> allClasses = finder.FillClassInfoWithTestInfo(TestsFileUtil.GetPathToTestsAssembly());
            int totalTests = allClasses.Sum(c => c.TestInfos.Count);
            Console.WriteLine($"\n=== Всего обнаружено тестов: {totalTests} ===");

            Console.WriteLine("\n========== ДЕМОНСТРАЦИЯ ФИЛЬТРАЦИИ ==========");

            var priorityFilter = TestFilter.ByPriority(1);
            List<ClassInfo> priorityFilteredClasses = CopyClassInfos(allClasses);
            finder.ApplyFilter(priorityFilteredClasses, priorityFilter);
            int priorityTests = priorityFilteredClasses.Sum(c => c.TestInfos.Count);
            Console.WriteLine($"После фильтра ByPriority(1): {priorityTests} тестов");
            foreach (var c in priorityFilteredClasses)
            {
                foreach (var t in c.TestInfos)
                {
                    Console.WriteLine($"  - {t.getNameOrDefault(t.MethodInfo.Name)} (Priority={t.getPriority()})");
                }
            }

            // 2. Фильтр: только НЕпараметризованные тесты
            var nonParamFilter = TestFilter.IsNonParameterized();
            List<ClassInfo> nonParamClasses = CopyClassInfos(allClasses);
            finder.ApplyFilter(nonParamClasses, nonParamFilter);
            int nonParamTests = nonParamClasses.Sum(c => c.TestInfos.Count);
            Console.WriteLine($"\nПосле фильтра IsNonParameterized(): {nonParamTests} тестов");
            foreach (var c in nonParamClasses)
            {
                foreach (var t in c.TestInfos)
                {
                    Console.WriteLine($"  - {t.getNameOrDefault(t.MethodInfo.Name)}");
                }
            }

            // 3. Фильтр: только параметризованные тесты
            var paramFilter = TestFilter.IsParameterized();
            List<ClassInfo> paramClasses = CopyClassInfos(allClasses);
            finder.ApplyFilter(paramClasses, paramFilter);
            int paramTests = paramClasses.Sum(c => c.TestInfos.Count);
            Console.WriteLine($"\nПосле фильтра IsParameterized(): {paramTests} тестов");
            foreach (var c in paramClasses)
            {
                foreach (var t in c.TestInfos)
                {
                    Console.WriteLine($"  - {t.getNameOrDefault(t.MethodInfo.Name)} (params: {string.Join(", ", t.getParametres() ?? Array.Empty<object>())})");
                }
            }

            // ============= ДЕМОНСТРАЦИЯ СОБЫТИЙ ПУЛА ПОТОКОВ =============
            Console.WriteLine("\n========== ДЕМОНСТРАЦИЯ СОБЫТИЙ ПУЛА ПОТОКОВ ==========");
            Console.WriteLine("Запускаем полную нагрузочную симуляцию с подпиской на события...");

            int minThreads = 2;
            TimeSpan idleTimeout = TimeSpan.FromSeconds(2);
            TimeSpan hangTimeout = TimeSpan.FromSeconds(5);
            TimeSpan taskTimeout = TimeSpan.FromSeconds(1);

            long poolRunTime;
            long syncRunTime;
            int repeatCount = 6;

            using (var pool = new DynamicThreadPool(minThreads, maxThreads, idleTimeout, hangTimeout, taskTimeout, logger))
            {
                subscribePoolOnEvents(pool);

                DynamicPoolTestRunner poolRunner = new DynamicPoolTestRunner(pool);

                LoadSimulator poolSimulator = new LoadSimulator(poolRunner, executor, logger);

                poolRunTime = RunXTimes(allClasses, poolSimulator, testLogger, repeatCount);

            }

            TestRunner testRunner = new TestRunner();
            LoadSimulator syncSimulator = new LoadSimulator(testRunner, executor, logger);
            syncRunTime = RunXTimes(allClasses, syncSimulator, testLogger, repeatCount);
            logger.LogEvent($"Thread Pool эффективнее синхронного запуска в {(double)syncRunTime / poolRunTime} раз");
            Console.ReadKey();
        }

        private static List<ClassInfo> CopyClassInfos(List<ClassInfo> source)
        {
            var result = new List<ClassInfo>();
            foreach (var c in source)
            {
                var copied = new ClassInfo(c.ClassType, (MiniTestFramework.Attributes.ClassAttributes.TestClassifierClassAttribute)c.ClassifierAttribute, new List<TestInfo>(c.TestInfos));
                result.Add(copied);
            }
            return result;
        }
    }
}
