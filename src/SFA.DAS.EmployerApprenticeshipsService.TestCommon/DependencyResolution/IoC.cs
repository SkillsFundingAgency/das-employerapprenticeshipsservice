using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
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
                c.Policies.Add<CurrentDatePolicy>();
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<LevyRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry(new DefaultRegistry(owinWrapper, cookieService, eventsApi, commitmentApi, messagePublisher));
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
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry(new DefaultRegistry(owinWrapper, cookieService, eventsApi, commitmentApi, messagePublisher));
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
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<LevyRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry(new LevyWorkerDefaultRegistry(hmrcService, messagePublisher, messageSubscriberFactory, eventsApi));
            });
        }
    }
}