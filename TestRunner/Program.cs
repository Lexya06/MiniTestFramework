using System.Reflection;
using MiniTestFramework.Attributes;
using MiniTestFramework.Exceptions;

namespace TestRunner
{
    class Program
    {
        static async Task Main()
        {
            var assembly = Assembly.Load("StudentManagementLibrary.Tests");

            foreach (var type in assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null))
            {
                var classAttr = type.GetCustomAttribute<TestClassAttribute>();
                TestLogger.Log($"Test class: {type.Name} — {classAttr?.Description}");

                bool shared = type.GetCustomAttribute<SharedContextAttribute>() != null;
                object? sharedInstance = shared ? Activator.CreateInstance(type) : null;

                var before = type.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<BeforeEachAttribute>() != null);

                var after = type.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<AfterEachAttribute>() != null);

                var testMethods = type.GetMethods()
                    .Select(m => new { Method = m, Attr = m.GetCustomAttribute<TestMethodAttribute>() })
                    .Where(x => x.Attr != null)
                    .OrderBy(x => x.Attr!.Priority)
                    .Select(x => x.Method);

                foreach (var method in testMethods)
                {
                    var instance = shared ? sharedInstance : Activator.CreateInstance(type);

                    try
                    {
                        before?.Invoke(instance, null);

                        var result = method.Invoke(instance, null);
                        if (result is Task task)
                            await task;

                        TestLogger.Log($"PASS: {method.Name}");
                    }
                    catch (Exception ex)
                    {
                        var inner = ex.InnerException ?? ex;
                        TestLogger.Log($"FAIL: {method.Name} — {inner.Message}");
                        throw new TestFailedException(method.Name, inner);
                    }
                    finally
                    {
                        after?.Invoke(instance, null);
                    }
                }
            }
        }
    }
}
