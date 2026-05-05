using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTestFramework.Attributes;

namespace TestRunner.Models
{
    public class BaseInfo
    {
        public BaseClassifierAttribute ClassifierAttribute { get; set; }
        public BaseInfo(Type type, BaseClassifierAttribute baseClassifierAttribute)
        {
            this.ClassifierAttribute = baseClassifierAttribute;
            this.ClassType = type;
        }
        public Type ClassType { get; set; }

        public string getNameOrDefault(string name)
        {
            if (this.ClassifierAttribute.Name == String.Empty)
            {
                return name;
            }
            else
                return this.ClassifierAttribute.Name;
        }

        public int getPriority()
        {
            return this.ClassifierAttribute.Priority;
        }

        public string getDescription()
        {
            return this.ClassifierAttribute.Description;
        }

        public override string ToString()
        {
            return $"{ClassType.Name}. {ClassifierAttribute.ToString()}";
        }
    }
}
