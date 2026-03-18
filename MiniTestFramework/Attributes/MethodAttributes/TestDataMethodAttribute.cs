using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes.MethodAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestDataMethodAttribute : BaseDataAttribute
    {
        public TestDataMethodAttribute(params object[] parameters) : base(parameters)
        {
        }
    }
}
