using Microsoft.Azure.ServiceBus;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ITopicClientFactory
{
    ITopicClient CreateClient(string connectionString, string messageGroupName);
}