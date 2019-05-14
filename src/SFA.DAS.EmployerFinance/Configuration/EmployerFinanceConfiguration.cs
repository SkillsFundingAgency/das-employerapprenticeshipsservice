using System;
using System.Linq.Expressions;
using SFA.DAS.Authentication;
using SFA.DAS.Caches;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class EmployerFinanceConfiguration : ITopicMessagePublisherConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
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
        public HmrcConfiguration Hmrc { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string LegacyServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString => LegacyServiceBusConnectionString;
        public string NServiceBusLicense { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public int FundsExpiryPeriod { get; set; }
    }
}