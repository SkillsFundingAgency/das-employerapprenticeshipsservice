using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerAccountsConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>("SFA.DAS.EmployerAccounts")).Singleton();
            For<EmployerFinanceConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerFinanceConfiguration>("SFA.DAS.EmployerFinance")).Singleton();
            For<ForecastingApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<ForecastingApiClientConfiguration>("SFA.DAS.Forecasting_api_client")).Singleton();
        }
    }
}