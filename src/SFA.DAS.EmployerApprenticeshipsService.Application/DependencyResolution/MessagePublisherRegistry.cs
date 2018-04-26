using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.NLog.Logger;
using StructureMap;
using Constants = SFA.DAS.EAS.Domain.Constants;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class MessagePublisherRegistry : Registry
    {
        public MessagePublisherRegistry()
        {
            Policies.Add(new TopicMessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName, Constants.ServiceVersion, new NLogLogger(typeof(TopicMessagePublisher))));
        }
    }
}