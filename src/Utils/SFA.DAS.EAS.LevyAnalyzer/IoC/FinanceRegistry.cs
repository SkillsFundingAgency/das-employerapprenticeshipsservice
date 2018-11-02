using SFA.DAS.EmployerFinance.Services;
using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyser.IoC
{
    public class FinanceRegistry : Registry
    {
        public FinanceRegistry()
        {
            For<IHmrcDateService>().Use<HmrcDateService>().Singleton();
        }
    }
}
