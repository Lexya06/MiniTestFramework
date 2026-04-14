using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRunner.Models;

namespace TestRunner.Runner
{
    public interface Runner
    {
        long RunTests(List<ClassInfo> classInfos, TestExecutor executor);
    }
}
