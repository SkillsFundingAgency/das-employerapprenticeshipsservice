﻿using SFA.DAS.Authentication;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using System;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class EmployerAccountsConfiguration : ITopicMessagePublisherConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public string CdnBaseUrl { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
        public string EmployerCommitmentsBaseUrl { get; set; }
        public string EmployerCommitmentsV2BaseUrl { get; set; }        
        public string EmployerFinanceBaseUrl { get; set; }
        public string EmployerIncentivesBaseUrl { get; set; }
        public string EmployerPortalBaseUrl { get; set; }
        public string EmployerProjectionsBaseUrl { get; set; }
        public string LevyTransferMatchingBaseUrl { get; set; }
        public string EmployerRecruitBaseUrl { get; set; }
        public string ReservationsBaseUrl { get; set; }
        public string EmployerFavouritesBaseUrl { get; set; }
        public string ProviderRelationshipsBaseUrl { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public PensionRegulatorConfiguration PensionRegulatorApi { get; set; }
        public ProviderRegistrationsConfiguration ProviderRegistrationsApi { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string LegacyServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString => LegacyServiceBusConnectionString;
        public string NServiceBusLicense { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringCharacters { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringSalt { get; set; }
        public string PublicHashstring { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public bool CanSkipRegistrationSteps { get; set; }
        public AccountApiConfiguration AccountApi { get; set; }
        public UserAornPayeLockConfiguration UserAornPayeLock { get; set; }
        public string ZenDeskHelpCentreUrl { get; set; }
        public string ReportTrainingProviderEmailAddress { get; set; }
        public string AdfsMetadata { get; set; }
        public string ZenDeskSnippetKey { get; set; }
        public string ZenDeskSectionId { get; set; }
        public string ZenDeskCobrowsingSnippetKey { get; set; }
        public CommitmentsApiV2ClientConfiguration CommitmentsApi { get; set; }
        public RecruitClientApiConfiguration RecruitApi { get; set; }
        public long DefaultServiceTimeoutMilliseconds { get; set; }
        public ContentClientApiConfiguration ContentApi { get; set; }
        public string ApplicationId { get; set; }
        public int DefaultCacheExpirationInMinutes { get; set; }
        public string SupportConsoleUsers { get; set; }
        public DateTime? LastTermsAndConditionsUpdate { get; set; }
    }
}