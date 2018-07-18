using Moq;
using SFA.DAS.Audit.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using StructureMap;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class EmployerAccountPayeControllerRegistry : Registry
    {
        public EmployerAccountPayeControllerRegistry(
            IMock<ICookieStorageService<EmployerAccountData>> cookieServiceEmployerAccountData,
            IMock<ICookieStorageService<FlashMessageViewModel>> cookieServiceFlashMessageViewModel,
            IMock<IAuthenticationService> owinWrapperMock, Mock<IEmployerCommitmentApi> commitmentsApi, Mock<IEventsApi> eventApi,
            IMock<IMessagePublisher> messagePublisher, Mock<IAuthorizationService> authorizationService)
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
            });

            For<IAuditApiClient>().Use<StubAuditApiClient>();
            For<IAuthenticationService>().Use(owinWrapperMock.Object);
            For<IEmployerCommitmentApi>().Use(commitmentsApi.Object);
            For<IEventsApi>().Use(() => eventApi.Object);
            For<IHashingService>().Use(new HashingService.HashingService("12345QWERTYUIOPNDGHAK", "TEST: Dummy hash code London is a city in UK"));
            For<ILog>().Use(Mock.Of<ILog>());
            For<IMessagePublisher>().Use(messagePublisher.Object);
            For<INotificationsApi>().Use(() => Mock.Of<INotificationsApi>());
            For<IPublicHashingService>().Use(x => new PublicHashingService("BCDEFGHIJKLMMOPQRSTUVWXYZ", "haShStRiNg"));
            For<IAuthorizationService>().Use(x => authorizationService.Object);
            For<ICookieStorageService<EmployerAccountData>>().Use(cookieServiceEmployerAccountData.Object);
            For<ICookieStorageService<FlashMessageViewModel>>().Use(cookieServiceFlashMessageViewModel.Object);
        }
    }
}