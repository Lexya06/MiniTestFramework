using System;

namespace MiniTestFramework.Attributes.MethodAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestDataSourceAttribute : Attribute
    {
        public string MethodName { get; }

        public TestDataSourceAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
