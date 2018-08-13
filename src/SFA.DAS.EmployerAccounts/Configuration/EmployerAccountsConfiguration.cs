using SFA.DAS.Authentication;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class EmployerAccountsConfiguration : ITopicMessagePublisherConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string LegacyServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString => LegacyServiceBusConnectionString;
        public string NServiceBusLicense { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}