using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class MessagingRegistry : Registry
    {
        public MessagingRegistry()
        {
            Policies.Add(new TopicMessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName, new NLogLogger(typeof(TopicMessagePublisher))));
        }
    }
}