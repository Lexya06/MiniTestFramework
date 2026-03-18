using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRunner.Models;

namespace TestRunner.Runner
{
    public class TestLogger
    {
        private const string LogFile = "test_results.txt";
        private StreamWriter GetFileWriter()
        {
            FileStream fs = new FileStream(LogFile, FileMode.Append, FileAccess.Write, FileShare.Read);
            return new StreamWriter(fs);
        }

        public void LogTestsInfo(List<ClassInfo> classInfos, string startMessage)
        {
            using (StreamWriter writer = GetFileWriter())
            {
                writer.WriteLine($"{startMessage}{Environment.NewLine}");
                writer.WriteLine($"{DateTime.Now} Test Results{Environment.NewLine}");
                foreach (ClassInfo classInfo in classInfos)
                {
                    writer.WriteLine($"{classInfo.ToString()}{Environment.NewLine}");
                }
            }
        }

       
    }
}
