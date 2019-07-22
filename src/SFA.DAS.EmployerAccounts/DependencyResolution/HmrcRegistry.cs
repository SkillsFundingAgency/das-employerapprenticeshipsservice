using SFA.DAS.EAS.Infrastructure.Interfaces.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class HmrcRegistry : Registry
    {
        public HmrcRegistry()
        {
            For<IHmrcConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().Hmrc).Singleton();
        }
    }
}
