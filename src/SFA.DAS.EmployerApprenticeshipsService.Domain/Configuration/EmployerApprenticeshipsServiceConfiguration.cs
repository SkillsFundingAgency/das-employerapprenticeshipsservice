using System.Collections.Generic;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : ITopicMessagePublisherConfiguration, ITopicMessageSubscriberConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
        public string EmployerCommitmentsBaseUrl { get; set; }
        public string EmployerFinanceBaseUrl { get; set; }
        public string EmployerPortalBaseUrl { get; set; }
        public string EmployerProjectionsBaseUrl { get; set; }
        public string EmployerRecruitBaseUrl { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public IdProcessorConfiguration IdProcessor { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
        public string NServiceBusLicense { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringCharacters { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringSalt { get; set; }
        public string PublicHashstring { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public Dictionary<string, string> ServiceBusConnectionStrings { get; set; }
        public EmployerAccountsApiConfiguration EmployerAccountsApi { get; set; }
        public EmployerFinanceApiConfiguration EmployerFinanceApi { get; set; }
    }
}