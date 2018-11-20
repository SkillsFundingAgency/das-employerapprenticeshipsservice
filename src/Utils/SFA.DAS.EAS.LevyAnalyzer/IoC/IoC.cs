using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyser.IoC
{
    public static class IoC
    {
        public static IContainer InitialiseIoC(string configLocation)
        {
            return new Container(c =>
            {
                c.AddRegistry(new CommonRegistry(configLocation));
                c.AddRegistry<FinanceRegistry>();
            });
        }
    }
}
