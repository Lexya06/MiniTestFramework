using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    public static class TestLogger
    {
        private const string LogFile = "test_results.txt";

        public static void Log(string message)
        {
            File.AppendAllText(LogFile,
                $"[{DateTime.Now}] {message}{Environment.NewLine}");
        }
    }
}
