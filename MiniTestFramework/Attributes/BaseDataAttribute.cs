using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestFramework.Attributes
{
    public class BaseDataAttribute : Attribute
    {
        public object[] Parametres {  get; set; }
        public BaseDataAttribute(params object[] parametres)
        {
            Parametres = parametres;
        }
    }
}
