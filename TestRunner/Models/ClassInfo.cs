using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Attributes;
using MiniTestFramework.Attributes.ClassAttributes;

namespace TestRunner.Models
{
    public class ClassInfo : BaseInfo
    {
        public ClassInfo(Type type, TestClassifierClassAttribute baseClassifierAttribute, List<TestInfo> testInfos) : base(type, baseClassifierAttribute)
        {
            this.TestInfos = new List<TestInfo>(testInfos);
        }

        public ConcurrentBag<TestResult> TestResults { get; } = new ConcurrentBag<TestResult>();
        public List<TestInfo> TestInfos { get; }

        public Boolean getShared()
        {
            return ((TestClassifierClassAttribute)this.ClassifierAttribute).IsShared;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{base.ToString()}{Environment.NewLine}");
            var testResultGroups = this.TestResults.GroupBy(t => t.TestInfo.getPriority()).OrderBy(g => g.Key);
            foreach (var testResultGroup in testResultGroups)
            {
                stringBuilder.AppendLine($"Test group priority:{testResultGroup.Key}");
                foreach (var testResult in testResultGroup)
                {
                    stringBuilder.AppendLine($"{testResult.ToString()}{Environment.NewLine}");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
