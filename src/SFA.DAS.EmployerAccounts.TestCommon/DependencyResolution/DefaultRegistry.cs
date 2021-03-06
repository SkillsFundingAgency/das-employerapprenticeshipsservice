﻿using Moq;
using SFA.DAS.Audit.Client;
using SFA.DAS.Authentication;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using StructureMap;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.TestCommon.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IAuthenticationService> owinWrapperMock, Mock<ICookieStorageService<EmployerAccountData>> cookieServiceMock, Mock<IEventsApi> eventApi, Mock<IEmployerCommitmentApi> commitmentsApi, Mock<IMessagePublisher> messagePublisher)
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(Validation.IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
            });

            For<IAuditApiClient>().Use<StubAuditApiClient>();
            For<IAuthenticationService>().Use(owinWrapperMock.Object);
            For<ICookieStorageService<EmployerAccountData>>().Use(cookieServiceMock.Object);
            For<IEmployerCommitmentApi>().Use(commitmentsApi.Object);
            For<IEventsApi>().Use(() => eventApi.Object);
            For<IHashingService>().Use(new HashingService.HashingService("12345QWERTYUIOPNDGHAK", "TEST: Dummy hash code London is a city in UK"));
            For<ILog>().Use(Mock.Of<ILog>());
            For<IMessagePublisher>().Use(messagePublisher.Object);
            For<INotificationsApi>().Use(() => Mock.Of<INotificationsApi>());
            For<IPublicHashingService>().Use(x => new HashingService.HashingService("BCDEFGHIJKLMMOPQRSTUVWXYZ", "haShStRiNg") as IPublicHashingService);
        }
    }
}