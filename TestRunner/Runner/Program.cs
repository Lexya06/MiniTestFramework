using System.Diagnostics;
using System.Reflection;
using MiniTestFramework.Attributes.ClassAttributes;
using MiniTestFramework.Attributes.MethodAttributes;
using MiniTestFramework.Exceptions;
using TestRunner.Models;
using TestRunner.Utils;

namespace TestRunner.Runner
{

    class Program
    {

        public static int GetMaxParallelThreads()
        {
            int minThreads = 1;
            int threads = 0;
            Boolean flag = false;
            do
            {
                Console.WriteLine("Enter max number of threads to parallel");
                if (int.TryParse(Console.ReadLine(), out threads) && threads >= minThreads)
                {
                    minThreads = threads;
                    flag = true;
                }
                else
                    Console.WriteLine("Number of threads is integer positive number greater zero. Please, try again");
            }while(!flag);
            return threads;
        }
        static async Task Main()
        {
            TestLogger logger = new TestLogger();
            TestRunner runner = new TestRunner();
            TestExecutor executor = new TestExecutor();
            TestFinder finder = new TestFinder();
            List<ClassInfo> classes = finder.FillClassInfoWithTestInfo(TestsFileUtil.GetPathToTestsAssembly());
            int threads = GetMaxParallelThreads();
            long asyncMillis = await runner.RunTestsAsync(classes, threads, executor);
            logger.LogTestsInfo(classes, $"\nParallel: {asyncMillis} ms");
            long syncMillis = runner.RunTestsSync(classes, executor);
            logger.LogTestsInfo(classes, $"\nSynchronic: {syncMillis} ms");

        }
    }
    
}
