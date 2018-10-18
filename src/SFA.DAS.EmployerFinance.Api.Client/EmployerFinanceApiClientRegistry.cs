using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClientRegistry : Registry
    {
        public EmployerFinanceApiClientRegistry()
        {
            For<EmployerFinanceApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerFinanceApiClientConfiguration>("SFA.DAS.EmployerFinance.Api.Client")).Singleton();
            For<IEmployerFinanceApiClient>().Use<EmployerFinanceApiClient>();
        }
    }
}