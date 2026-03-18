using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner.Models
{
    public class TestResult
    {
        public TestResult(TestInfo testInfo, string status)
        {
            this.TestInfo = testInfo;
            this.Status = status;
        }
        public TestInfo TestInfo { get; set; }
        public string Status {  get; set; }
        public Exception? Ex { get; set; }

        public override string ToString()
        {
            string info = $"{Status}. {TestInfo.ToString()}";
            if (Ex != null)
            {
                info += $"Error message = {Ex.Message} Error type = {Ex.GetType().Name}.";
            }
            return info ;
           
        }
    }
}
