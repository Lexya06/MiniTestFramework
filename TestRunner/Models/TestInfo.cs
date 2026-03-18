using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Attributes;
using MiniTestFramework.Attributes.MethodAttributes;

namespace TestRunner.Models
{
    public class TestInfo : BaseInfo
    {
        public TestInfo(Type type, TestClassifierMethodAttribute testClassifierAttribute, MethodInfo methodInfo, TestDataMethodAttribute? testData, List<MethodInfo> beforeEach, List<MethodInfo> afterEach): base(type, testClassifierAttribute)
        {
            this.Data = testData;
            this.BeforeEach = new List<MethodInfo>(beforeEach);
            this.AfterEach = new List<MethodInfo>(afterEach);
            this.MethodInfo = methodInfo;

        }

        public TestDataMethodAttribute? Data { private get; set; }
        public MethodInfo MethodInfo { get; set; }
        public List<MethodInfo> BeforeEach { get; }
        public List<MethodInfo> AfterEach { get; }

        public object[]? getParametres()
        {
            return Data?.Parametres;
        }
        public double? getTimeout()
        {
            return ((TestClassifierMethodAttribute)this.ClassifierAttribute).Timeout;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
