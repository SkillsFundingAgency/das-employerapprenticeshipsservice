using SFA.DAS.Authentication;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class EmployerFinanceConfiguration : ITopicMessagePublisherConfiguration
    {
        public string ApplicationId { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string CdnBaseUrl { get; set; }
        public EmployerFinanceOuterApiConfiguration EmployerFinanceOuterApiConfiguration { get; set; }
        public ContentClientApiConfiguration ContentApi { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public int DefaultCacheExpirationInMinutes { get; set; }
        public decimal TransferAllowancePercentage { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
        public string EmployerCommitmentsBaseUrl { get; set; }
        public string EmployerCommitmentsV2BaseUrl { get; set; }
        public string EmployerFinanceBaseUrl { get; set; }
        public string EmployerPortalBaseUrl { get; set; }
        public string EmployerProjectionsBaseUrl { get; set; }
        public string EmployerRecruitBaseUrl { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string LegacyServiceBusConnectionString { get; set; }
        public string LevyTransferMatchingBaseUrl { get; set; }
        public string MessageServiceBusConnectionString => LegacyServiceBusConnectionString;
        public string NServiceBusLicense { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public virtual int FundsExpiryPeriod { get; set; }
        public AccountApiConfiguration AccountApi { get; set; }
        public TokenServiceApiClientConfiguration TokenServiceApi { get; set; }
        public CommitmentsApiV2ClientConfiguration CommitmentsApi { get; set; }
        public PaymentsApiClientConfiguration PaymentsEventsApi { get; set; }
        public string ReservationsBaseUrl { get; set; }
        public string ZenDeskHelpCentreUrl { get; set; }
        public string ZenDeskSnippetKey { get; set; }
        public string ZenDeskSectionId { get; set; }
        public string ZenDeskCobrowsingSnippetKey { get; set; }
        public EmployerFinanceApiClientConfiguration EmployerFinanceApi { get; set; }
    }
}