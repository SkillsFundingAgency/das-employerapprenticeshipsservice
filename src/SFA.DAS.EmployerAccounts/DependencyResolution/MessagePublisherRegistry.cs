using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class MessagePublisherRegistry : Registry
{
    public MessagePublisherRegistry()
    {
        Policies.Add(new TopicMessagePublisherPolicy<EmployerAccountsConfiguration>("SFA.DAS.EmployerAccounts", "1.0", new NLogLogger(typeof(TopicMessagePublisher))));
    }
}