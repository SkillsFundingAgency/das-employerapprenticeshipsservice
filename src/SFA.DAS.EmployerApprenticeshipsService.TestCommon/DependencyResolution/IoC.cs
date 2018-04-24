using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.TestCommon.MockPolicy;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public static class IoC
    {
        public const string EmployerApprenticeshipConfigurationName = "SFA.DAS.EmployerApprenticeshipsService";
        public const string LevyAggregationProviderName = "SFA.DAS.LevyAggregationProvider";
        public const string AuditApiClientConfigurationName = "SFA.DAS.AuditApiClient";
        public const string TokenServiceApiConfigurationName = "SFA.DAS.TokenServiceApiClient";

        public static Container CreateContainer(
            Mock<IMessagePublisher> messagePublisher,
            Mock<IAuthenticationService> owinWrapper,
            Mock<ICookieStorageService<EmployerAccountData>> cookieService,
            Mock<IEventsApi> eventsApi,
            Mock<IEmployerCommitmentApi> commitmentApi)
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(EmployerApprenticeshipConfigurationName));
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>(LevyAggregationProviderName));
                c.Policies.Add(new ConfigurationPolicy<AuditApiClientConfiguration>(AuditApiClientConfigurationName));
                c.Policies.Add<CurrentDatePolicy>();
                c.Policies.Add(new MockMessagePublisherPolicy(messagePublisher));
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry(new DefaultRegistry(owinWrapper, cookieService, eventsApi, commitmentApi));
            });
        }

        public static Container CreateContainer(
            Mock<IMessagePublisher> messagePublisher,
            Mock<IAuthenticationService> owinWrapper,
            Mock<ICookieStorageService<EmployerAccountData>> cookieService,
            Mock<IEventsApi> eventsApi,
            Mock<IEmployerCommitmentApi> commitmentApi,
            LevyDeclarationProviderConfiguration levyDeclarationProviderConfiguration)
        {
            var container = new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(EmployerApprenticeshipConfigurationName));
                c.Policies.Add(new ConfigurationPolicy<AuditApiClientConfiguration>(AuditApiClientConfigurationName));
                c.Policies.Add<CurrentDatePolicy>();
                c.Policies.Add(new MockMessagePublisherPolicy(messagePublisher));
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry(new DefaultRegistry(owinWrapper, cookieService, eventsApi, commitmentApi));
            });

            container.Inject(levyDeclarationProviderConfiguration);

            return container;
        }

        public static Container CreateLevyWorkerContainer(
            Mock<IMessagePublisher> messagePublisher,
            Mock<IMessageSubscriberFactory> messageSubscriberFactory,
            IHmrcService hmrcService,
            IEventsApi eventsApi = null)
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>(LevyAggregationProviderName));
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(EmployerApprenticeshipConfigurationName));
                c.Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>(TokenServiceApiConfigurationName));
                c.Policies.Add(new MockMessagePublisherPolicy(messagePublisher));
                c.Policies.Add(new MockMessageSubscriberPolicy(messageSubscriberFactory));
                c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry(new LevyWorkerDefaultRegistry(hmrcService, eventsApi));
            });
        }
    }
}
