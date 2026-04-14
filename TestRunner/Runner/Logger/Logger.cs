using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Runner
{
   
    public class Logger
    {
        protected object _lock = new object();
        public void LogEvent(string message)
        {
            lock (_lock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [INFO] {message}{Environment.NewLine}");
            }
        }


        public void LogError(string error)
        {
            lock (_lock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ERROR] {error}{Environment.NewLine}");
            }

        }
    }
}
