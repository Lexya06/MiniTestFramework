using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Exceptions
{
    public class TestFailedException : Exception
    {
        public string TestName { get; }
        public string ClassName { get; }

        public TestFailedException(string testName, string className, Exception inner)
            : base($"Test '{testName}' in '{className}' failed", inner)
        {
            TestName = testName;
            ClassName = className;
        }
    }
}
