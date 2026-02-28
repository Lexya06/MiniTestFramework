using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttribute : Attribute
    {
        public string Description { get; }

        public TestClassAttribute(string description = "")
        {
            Description = description;
        }
    }
}
