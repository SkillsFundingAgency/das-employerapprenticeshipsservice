using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerApprenticeshipsServiceConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName)).Singleton();
            For<FeaturesConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<FeaturesConfiguration>($"{Constants.ServiceName}.FeaturesV2")).Singleton();
        }
    }
}