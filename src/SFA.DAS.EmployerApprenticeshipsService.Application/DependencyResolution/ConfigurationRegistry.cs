using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<FeaturesConfiguration>().Use(c => ConfigurationHelper.GetConfiguration<FeaturesConfiguration>("SFA.DAS.EmployerApprenticeshipsService.FeaturesV2")).Singleton();
            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            Policies.Add(new ConfigurationPolicy<AuditApiClientConfiguration>("SFA.DAS.AuditApiClient"));
            Policies.Add(new ConfigurationPolicy<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI"));
            Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName));
            Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
            Policies.Add(new ConfigurationPolicy<NotificationsApiClientConfiguration>($"{Constants.ServiceName}.Notifications"));
            Policies.Add(new ConfigurationPolicy<ReferenceDataApiClientConfiguration>("SFA.DAS.ReferenceDataApiClient"));
            Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient"));
        }
    }
}