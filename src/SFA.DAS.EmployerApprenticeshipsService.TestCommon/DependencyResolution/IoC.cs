﻿using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.EnvironmentInfo;
using SFA.DAS.EAS.TestCommon.MockPolicy;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using StructureMap;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public static class IoC
    {
        public static Container CreateContainer(Mock<IMessagePublisher> messagePublisher, Mock<IOwinWrapper> owinWrapper, Mock<ICookieStorageService<EmployerAccountData>> cookieService, Mock<IEventsApi> eventsApi, Mock<IEmployerCommitmentApi> commitmentApi)
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService", new ConfigurationInfo<EmployerApprenticeshipsServiceConfiguration>()));
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider", new ConfigurationInfo<LevyDeclarationProviderConfiguration>()));
                c.Policies.Add(new ConfigurationPolicy<AuditApiClientConfiguration>("SFA.DAS.AuditApiClient", new ConfigurationInfo<AuditApiClientConfiguration>()));
                c.Policies.Add<CurrentDatePolicy>();
                c.Policies.Add(new MockMessagePolicy(messagePublisher));
                c.AddRegistry(new DefaultRegistry(owinWrapper, cookieService, eventsApi, commitmentApi));
            });
        }

        public static Container CreateLevyWorkerContainer(IMessagePublisher messagePublisher, IPollingMessageReceiver messageReceiver, IHmrcService hmrcService, IEventsApi eventsApi = null)
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider", new ConfigurationInfo<LevyDeclarationProviderConfiguration>()));
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService", new ConfigurationInfo<EmployerApprenticeshipsServiceConfiguration>()));
                c.Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient", new ConfigurationInfo<TokenServiceApiClientConfiguration>()));
                c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry(new LevyWorkerDefaultRegistry(messagePublisher, messageReceiver, hmrcService, eventsApi));
            });
        }
    }
}
