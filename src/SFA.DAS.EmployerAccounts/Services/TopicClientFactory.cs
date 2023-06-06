using Microsoft.Azure.ServiceBus;

namespace SFA.DAS.EmployerAccounts.Services;

public class TopicClientFactory : ITopicClientFactory
{
    public ITopicClient CreateClient(string connectionString, string messageGroupName)
    {
        return new TopicClient(connectionString, messageGroupName);
    }
}