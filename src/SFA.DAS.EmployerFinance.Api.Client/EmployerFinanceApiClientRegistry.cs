using SFA.DAS.AutoConfiguration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClientRegistry : Registry
    {
        public EmployerFinanceApiClientRegistry()
        {
            For<EmployerFinanceApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFinanceApiClientConfiguration>("SFA.DAS.EmployerFinance.Api.Client")).Singleton();
            For<IEmployerFinanceApiClient>().Use<EmployerFinanceApiClient>();
        }
    }
}