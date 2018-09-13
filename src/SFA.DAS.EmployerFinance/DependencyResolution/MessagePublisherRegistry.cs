using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class MessagePublisherRegistry : Registry
    {
        public MessagePublisherRegistry()
        {
            Policies.Add(new TopicMessagePublisherPolicy<EmployerFinanceConfiguration>("SFA.DAS.EmployerFinance", "1.0", new NLogLogger(typeof(TopicMessagePublisher))));
        }
    }
}