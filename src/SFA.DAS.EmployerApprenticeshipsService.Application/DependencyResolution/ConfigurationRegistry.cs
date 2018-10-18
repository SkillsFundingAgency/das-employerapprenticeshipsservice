using SFA.DAS.Configuration;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerApprenticeshipsServiceConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName)).Singleton();
            For<LevyDeclarationProviderConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider")).Singleton();
            For<PaymentProviderConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<PaymentProviderConfiguration>("SFA.DAS.PaymentProvider")).Singleton();
        }
    }
}