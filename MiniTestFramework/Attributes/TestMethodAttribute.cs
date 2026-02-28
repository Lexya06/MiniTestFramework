using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestMethodAttribute : Attribute
    {
        public int Priority { get; }

        public TestMethodAttribute(int priority = 0)
        {
            Priority = priority;
        }
    }
}
