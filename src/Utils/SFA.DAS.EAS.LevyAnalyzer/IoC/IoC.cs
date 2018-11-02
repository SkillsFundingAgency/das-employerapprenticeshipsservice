using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyser.IoC
{
    public static class IoC
    {
        public static IContainer InitialiseIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<CommonRegistry>();
                c.AddRegistry<FinanceRegistry>();
            });
        }
    }
}
