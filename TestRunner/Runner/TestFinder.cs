using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Attributes.ClassAttributes;
using MiniTestFramework.Attributes.MethodAttributes;
using TestRunner.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestRunner.Runner
{
    public class TestFinder
    {
        List<TestInfo> FindTests(Type type, List<MethodInfo> beforeEach, List<MethodInfo> afterEach)
        {
            List<TestInfo> testInfos = new List<TestInfo>();
            foreach (var method in type.GetMethods())
            {
                var testClassifier = method.GetCustomAttribute<TestClassifierMethodAttribute>();
                if (testClassifier != null)
                {
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
            testInfos.OrderBy(t => t.getPriority());
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
    }
}
