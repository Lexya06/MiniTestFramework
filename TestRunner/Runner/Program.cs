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

        static void Main()
        {
            TestLogger testLogger = new TestLogger();
            Logger logger = new Logger();
            TestExecutor executor = new TestExecutor();
            TestFinder finder = new TestFinder();

            // Ввод данных пользователем
            int maxThreads = GetMaxParallelThreads();

            List<ClassInfo> classes = finder.FillClassInfoWithTestInfo(TestsFileUtil.GetPathToTestsAssembly());

            int minThreads = 2;
            TimeSpan idleTimeout = TimeSpan.FromSeconds(2);
            TimeSpan hangTimeout = TimeSpan.FromSeconds(5);
            TimeSpan taskTimeout = TimeSpan.FromSeconds(1);

            long poolRunTime;
            long syncRunTime;
            int repeatCount = 5;

            using (var pool = new DynamicThreadPool(minThreads, maxThreads, idleTimeout, hangTimeout, taskTimeout, logger))
            {

                DynamicPoolTestRunner poolRunner = new DynamicPoolTestRunner(pool);

                LoadSimulator poolSimulator = new LoadSimulator(poolRunner, executor, logger);

                poolRunTime = RunXTimes(classes, poolSimulator, testLogger, repeatCount);
               
            }
            
            TestRunner testRunner = new TestRunner();
            LoadSimulator syncSimulator = new LoadSimulator(testRunner, executor, logger);
            syncRunTime = RunXTimes(classes, syncSimulator, testLogger, repeatCount);
            logger.LogEvent($"Thread Pool эффективнее синхронного запуска в {(double)syncRunTime / poolRunTime} раз");
            Console.ReadKey();
        }

        
    }
}
