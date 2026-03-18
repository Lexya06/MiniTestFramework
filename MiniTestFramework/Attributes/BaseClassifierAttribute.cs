using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes
{
   
    public class BaseClassifierAttribute:Attribute
    {
        public string Name { set; get; }
        public int Priority { get; set; }
        public string Description { set; get; }



        
        public BaseClassifierAttribute(string name, int priority, string description) 
        {
            Name = name;
            Priority = priority;
            Description = description;
        }

       

        public override string ToString()
        {
            string info = $"Name = {Name}. Priority = {Priority}.";
            if (Description != String.Empty)
            {
                info += $" Description = {Description}.";
            }
            return info;
        }
    }
}
