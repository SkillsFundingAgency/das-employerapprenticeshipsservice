using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.NLog.Logger;
using StructureMap;
using Constants = SFA.DAS.EAS.Domain.Constants;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName));
                c.Policies.Add(new TopicMessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName, Constants.ServiceVersion, new NLogLogger(typeof(TopicMessagePublisher))));
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}