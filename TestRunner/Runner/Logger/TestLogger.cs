using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRunner.Models;

namespace TestRunner.Runner
{
   
        public class TestLogger : Logger
        {
            private const string LogFile = "test_results.txt";
            private readonly StreamWriter _writer;

            public TestLogger()
            {
                _writer = new StreamWriter(
                    new FileStream(LogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                )
                {
                    AutoFlush = true
                };
            }

            ~TestLogger()
            {
                _writer.Close();
            }

        

            public void LogTestsInfo(List<ClassInfo> classInfos, string startMessage)
            {

                lock (_lock)
                {
                    
                   
                    LogTestsInfo(startMessage);
                    _writer.WriteLine($"{DateTime.Now} Test Results{Environment.NewLine}");
                    foreach (ClassInfo classInfo in classInfos)
                    {
                        _writer.WriteLine($"{classInfo.ToString()}{Environment.NewLine}");
                    }
                    
                }
            }

            public void LogTestsInfo(string startMessage)
            {
                lock (_lock)
                {
                   
                     _writer.WriteLine($"{startMessage}{Environment.NewLine}");
                   
                }
            }

        }
    
}
