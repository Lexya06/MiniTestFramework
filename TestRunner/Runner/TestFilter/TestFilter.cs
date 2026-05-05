using System;
using TestRunner.Models;

namespace TestRunner.Runner
{
    public static class TestFilter
    {
        public static Predicate<TestInfo> ByPriority(int priority)
        {
            return test => test.getPriority() == priority;
        }

        public static Predicate<TestInfo> ByMinPriority(int minPriority)
        {
            return test => test.getPriority() >= minPriority;
        }

        public static Predicate<TestInfo> ByMaxPriority(int maxPriority)
        {
            return test => test.getPriority() <= maxPriority;
        }

        public static Predicate<TestInfo> IsParameterized()
        {
            return test => test.getParametres() != null && test.getParametres()!.Length > 0;
        }

        public static Predicate<TestInfo> IsNonParameterized()
        {
            return test => test.getParametres() == null || test.getParametres()!.Length == 0;
        }
    }
}
