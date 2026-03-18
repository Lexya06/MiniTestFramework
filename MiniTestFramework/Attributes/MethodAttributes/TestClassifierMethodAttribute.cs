using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes.MethodAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestClassifierMethodAttribute : BaseClassifierAttribute
    {
        public long? Timeout { get; set; }
       

        public TestClassifierMethodAttribute(string name = "", int priority = 0, string description = "") : base(name, priority, description)
        {
            this.Timeout = null;
        }

        public TestClassifierMethodAttribute(long timeout, string name = "", int priority = 0, string description = "") : base(name, priority, description)
        {
            this.Timeout = timeout;
        }


    }
}
