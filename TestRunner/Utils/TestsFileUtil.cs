using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Utils
{
    public static class TestsFileUtil
    {
        public static string GetPathToTestsAssembly()
        {
            string? path = null;
            do
            {
                Console.WriteLine("Enter path to tests assembly");
                path = Console.ReadLine();
                if (!File.Exists(path))
                {
                    path = null;
                    Console.WriteLine($"File with path {path} not exists. Please, try again");
                }
            } while (path == null);
            return path;

        }
    }
}
