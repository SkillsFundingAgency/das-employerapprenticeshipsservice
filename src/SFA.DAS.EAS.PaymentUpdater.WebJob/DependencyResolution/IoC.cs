using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using StructureMap;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.DependencyResolution
{
    public static class IoC
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";

        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
                c.Policies.Add(new ConfigurationPolicy<PaymentsApiClientConfiguration>("SFA.DAS.PaymentsAPI"));
                c.Policies.Add(new TopicMessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
