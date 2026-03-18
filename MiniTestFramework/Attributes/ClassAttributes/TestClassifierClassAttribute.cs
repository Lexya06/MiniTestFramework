using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes.ClassAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassifierClassAttribute : BaseClassifierAttribute
    {
        public Boolean IsShared {  get; set; }

        public TestClassifierClassAttribute(string name="", int priority=0, string description="", bool isShared = false) : base(name, priority, description)
        {
            
            IsShared = isShared;
        }


        public override string ToString()
        {
            return $"Shared context: {IsShared}." + base.ToString();
        }
    }

    
}
