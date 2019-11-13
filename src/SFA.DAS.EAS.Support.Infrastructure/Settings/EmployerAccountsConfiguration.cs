using SFA.DAS.Messaging.AzureServiceBus.StructureMap;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings
{
    public class EmployerAccountsConfiguration : ITopicMessagePublisherConfiguration
    {
        public string EmployerAccountsBaseUrl { get; set; }
        public string LegacyServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString => LegacyServiceBusConnectionString;

    }
}