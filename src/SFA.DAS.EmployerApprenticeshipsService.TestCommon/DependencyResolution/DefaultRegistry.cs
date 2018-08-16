using Moq;
using SFA.DAS.Audit.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using StructureMap;
using SFA.DAS.HashingService;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Interfaces;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IAuthenticationService> owinWrapperMock, Mock<ICookieStorageService<EmployerAccountData>> cookieServiceMock, Mock<IEventsApi> eventApi, Mock<IEmployerCommitmentApi> commitmentsApi, Mock<IMessagePublisher> messagePublisher)
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
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
            For<IPublicHashingService>().Use(x => new PublicHashingService("BCDEFGHIJKLMMOPQRSTUVWXYZ", "haShStRiNg"));
        }
    }
}