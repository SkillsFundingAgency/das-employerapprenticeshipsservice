using SFA.DAS.EAS.Infrastructure.Interfaces.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class HmrcRegistry : Registry
    {
        public HmrcRegistry()
        {
            For<IHmrcConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().Hmrc).Singleton();
        }
    }
}
