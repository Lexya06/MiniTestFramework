using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Exceptions
{
     public class TestAssertionException : Exception
     {
        public string? Expected { get; }
        public string? Actual { get; }


        public TestAssertionException(string message,
                                      string? expected = null,
                                      string? actual = null)
        : base(BuildMessage(message, expected, actual))   
        {
            
            Expected = expected;
            Actual = actual;
        }

        private static string BuildMessage(string message, string? expected, string? actual)
        {
            var result = message;
            if (expected != null)
                result += $" Expected: {expected}.";
            if (actual != null)
                result += $" Actual: {actual}.";
            return result;
        }
    }
}
