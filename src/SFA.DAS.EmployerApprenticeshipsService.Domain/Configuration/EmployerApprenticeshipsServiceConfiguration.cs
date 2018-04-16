using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : IConfiguration, ITopicMessagePublisherConfiguration, ITopicMessageSubscriberConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
        public PostcodeAnywhereConfiguration PostcodeAnywhere { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public Dictionary<string, string> ServiceBusConnectionStrings { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
    }
}