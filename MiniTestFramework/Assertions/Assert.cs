using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Exceptions;

namespace MiniTestFramework.Assertions
{
    public static class Assert
    {
        public static void IsTrue(bool condition)
        {
            if (!condition)
                throw new TestAssertionException("Condition is false");
        }

        public static void IsFalse(bool condition)
        {
            if (condition)
                throw new TestAssertionException("Condition is true");
        }

        
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!Equals(expected, actual))
                throw new TestAssertionException(
                    "Values are not equal",
                    expected?.ToString(),
                    actual?.ToString());
        }


        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            if (Equals(notExpected, actual))
                throw new TestAssertionException("Values should not be equal");
        }

        public static void IsNull(object? obj)
        {
            if (obj != null)
                throw new TestAssertionException("Object is not null");
        }

        public static void IsNotNull(object? obj)
        {
            if (obj == null)
                throw new TestAssertionException("Object is null");
        }

        public static void Greater(double? a, double? b)
        {
            IsNotNull(a);
            IsNotNull(b);
            if (a <= b)
                throw new TestAssertionException("Value is not greater");
        }

        public static void Less(double? a, double? b)
        {
            IsNotNull(a);
            IsNotNull(b);
            if (a >= b)
                throw new TestAssertionException("Value is not less");
        }

        public static void Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
                throw new TestAssertionException("Exception was not thrown");
            }
            catch (T) { 
            }
        }

        public static void Fail(string message)
        {
            throw new TestAssertionException(message);
        }
    }
}
