using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyzer.IoC
{
    public static class IoC
    {
        public static IContainer InitialiseIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<CommonRegistry>();
            });
        }
    }
}
