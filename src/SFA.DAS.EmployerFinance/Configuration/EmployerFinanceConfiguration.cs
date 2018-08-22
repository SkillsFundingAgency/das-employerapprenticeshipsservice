﻿using SFA.DAS.Authenication;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class EmployerFinanceConfiguration : ITopicMessagePublisherConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string LegacyServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString => LegacyServiceBusConnectionString;
        public string NServiceBusLicense { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}