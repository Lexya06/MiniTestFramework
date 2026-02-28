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

        public TestFailedException(string testName, Exception inner)
            : base($"Test '{testName}' failed", inner)
        {
            TestName = testName;
        }
    }
}
