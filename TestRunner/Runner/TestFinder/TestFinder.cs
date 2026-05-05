using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Attributes.ClassAttributes;
using MiniTestFramework.Attributes.MethodAttributes;
using TestRunner.Models;

namespace TestRunner.Runner
{
    public class TestFinder
    {
        private List<TestInfo> FindTests(Type type, List<MethodInfo> beforeEach, List<MethodInfo> afterEach)
        {
            List<TestInfo> testInfos = new List<TestInfo>();
            foreach (var method in type.GetMethods())
            {
                var testClassifier = method.GetCustomAttribute<TestClassifierMethodAttribute>();
                if (testClassifier != null)
                {
                    // TestDataMethodAttribute (inline parameters)
                    var testData = method.GetCustomAttributes<TestDataMethodAttribute>().ToList();
                    if (testData.Any())
                    {
                        foreach (var data in testData)
                        {
                            testInfos.Add(new TestInfo(
                                type,
                                testClassifier,
                                method,
                                data,
                                beforeEach,
                                afterEach
                                )
                            );
                        }
                    }
                    // TestDataSourceAttribute (yield return data source)
                    else if (method.GetCustomAttribute<TestDataSourceAttribute>() is TestDataSourceAttribute sourceAttr)
                    {
                        var sourceMethod = type.GetMethod(sourceAttr.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        if (sourceMethod != null)
                        {
                            object? instance = sourceMethod.IsStatic ? null : Activator.CreateInstance(type);
                            var dataEnumerable = sourceMethod.Invoke(instance, null) as System.Collections.IEnumerable;
                            if (dataEnumerable != null)
                            {
                                foreach (var item in dataEnumerable)
                                {
                                    var parameters = item as object[];
                                    var dataAttr = new TestDataMethodAttribute(parameters ?? Array.Empty<object>());
                                    testInfos.Add(new TestInfo(
                                        type,
                                        testClassifier,
                                        method,
                                        dataAttr,
                                        beforeEach,
                                        afterEach
                                    ));
                                }
                            }
                        }
                    }
                    else
                    {
                        testInfos.Add(new TestInfo(
                            type,
                            testClassifier,
                            method,
                            null,
                            beforeEach,
                            afterEach
                            )
                        );
                    }

                }
            }
            return testInfos;
        }

        public List<ClassInfo> FillClassInfoWithTestInfo(string assemblyPath)
        {
            List<ClassInfo> classInfos = new List<ClassInfo>();
            var assembly = Assembly.LoadFrom(assemblyPath);
            var classes = assembly.GetTypes();
            foreach (var type in classes)
            {
                TestClassifierClassAttribute? testClassifierClassAttribute = type.GetCustomAttribute<TestClassifierClassAttribute>();
                if (testClassifierClassAttribute != null)
                {
                    List<MethodInfo> beforeEach = type.GetMethods().Where(x => x.GetCustomAttribute<BeforeEachAttribute>() != null && x.GetCustomAttribute<TestClassifierMethodAttribute>() == null).ToList();
                    List<MethodInfo> afterEach = type.GetMethods().Where(x => x.GetCustomAttribute<AfterEachAttribute>() != null && x.GetCustomAttribute<TestClassifierMethodAttribute>() == null).ToList();
                    List<TestInfo> testInfos = FindTests(type, beforeEach, afterEach);
                    ClassInfo classInfo = new(
                        type,
                        testClassifierClassAttribute,
                        testInfos
                    );
                    classInfos.Add(classInfo);
                }

            }
            classInfos.OrderBy(c => c.getPriority());
            return classInfos;
        }

        public void ApplyFilter(List<ClassInfo> classInfos, Predicate<TestInfo> testFilter)
        {
            foreach (var classInfo in classInfos)
            {
                classInfo.TestInfos.RemoveAll(t => !testFilter(t));
            }
            classInfos.RemoveAll(c => c.TestInfos.Count == 0);
        }
    }
}
