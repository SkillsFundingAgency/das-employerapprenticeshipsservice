using BoDi;
using HMRC.ESFA.Levy.Api.Client;
using MediatR;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.AccountTeam;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class ObjectContainerExtensions
    {
        public static IObjectContainer AddImplementationsRequiredInCommonSteps(this IObjectContainer objectContainer, IContainer container)
        {
            //objectContainer.RegisterInstanceAs(container.GetInstance<EAS.Infrastructure.Data.EmployerAccountsDbContext>());
            //objectContainer.RegisterInstanceAs(container.GetInstance<EAS.Infrastructure.Data.EmployerFinanceDbContext>());

            objectContainer.RegisterInstanceAs(container.GetInstance<IHashingService>());
            objectContainer.RegisterInstanceAs(container.GetInstance<IMediator>());
            //objectContainer.RegisterInstanceAs(container.GetInstance<IPublicHashingService>());
            objectContainer.RegisterInstanceAs(container.GetInstance<ILog>());

            objectContainer.RegisterInstanceAs(new Mock<IEventsApi>());
            objectContainer.RegisterInstanceAs(new Mock<IApprenticeshipLevyApiClient>());
            objectContainer.RegisterInstanceAs(new Mock<IAuthenticationService>());
            objectContainer.RegisterInstanceAs(new Mock<ICurrentDateTime>());
            objectContainer.RegisterInstanceAs(new Mock<ICurrentDateTime>());
            //objectContainer.RegisterInstanceAs(new Mock<ICookieStorageService<FlashMessageViewModel>>());
            //objectContainer.RegisterInstanceAs(new Mock<ICookieStorageService<EmployerAccountData>>());
            objectContainer.RegisterInstanceAs(new Mock<IAuthorizationService>());
            objectContainer.RegisterInstanceAs(new Mock<IPayeRepository>());
            objectContainer.RegisterInstanceAs(new Mock<IMembershipRepository>());
            objectContainer.RegisterInstanceAs(new Mock<IEmployerAccountRepository>());

            container.Configure(c =>
            {
                c.For<IApprenticeshipLevyApiClient>().Use(objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>().Object);
                c.For<IAuthenticationService>().Use(objectContainer.Resolve<Mock<IAuthenticationService>>().Object);
                c.For<IAuthorizationService>().Use(objectContainer.Resolve<Mock<IAuthorizationService>>().Object);
                //c.For<ICookieStorageService<EmployerAccountData>>().Use(objectContainer.Resolve<Mock<ICookieStorageService<EmployerAccountData>>>().Object);
                //c.For<ICookieStorageService<FlashMessageViewModel>>().Use(objectContainer.Resolve<Mock<ICookieStorageService<FlashMessageViewModel>>>().Object);
                c.For<ICurrentDateTime>().Use(objectContainer.Resolve<Mock<ICurrentDateTime>>().Object);
                c.For<ICurrentDateTime>().Use(objectContainer.Resolve<Mock<ICurrentDateTime>>().Object);
                c.For<IEventsApi>().Use(objectContainer.Resolve<Mock<IEventsApi>>().Object);
                c.For<IPayeRepository>().Use(objectContainer.Resolve<Mock<IPayeRepository>>().Object);
                c.For<IMembershipRepository>().Use(objectContainer.Resolve<Mock<IMembershipRepository>>().Object);
                c.For<IEmployerAccountRepository>().Use(objectContainer.Resolve<Mock<IEmployerAccountRepository>>().Object);
            });

            return objectContainer;
        }

        public static IObjectContainer SetupAuthorizedUser(this IObjectContainer objectContainer, Account account)
        {
            objectContainer.Resolve<Mock<IMembershipRepository>>().Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string hashedAccountId, string externalUserId) => new MembershipView
                {
                    //HashedAccountId = hashedAccountId,
                    //UserRef = externalUserId
                });

            objectContainer.Resolve<Mock<IEmployerAccountRepository>>()
                .Setup(x => x.GetAccountByHashedId(It.IsAny<string>()))
                .ReturnsAsync(account);

            return objectContainer;
        }

        public static IObjectContainer SetupEatOrchestrator(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstanceAs(new EmployerAccountTransactionsOrchestrator(
                container.GetInstance<IMediator>(),
                container.GetInstance<ICurrentDateTime>(), container.GetInstance<ILog>()));

            return objectContainer;
        }

        public static IObjectContainer SetupEatController(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstanceAs(new EmployerAccountTransactionsController(
                container.GetInstance<IAuthenticationService>(),
                container.GetInstance<EmployerAccountTransactionsOrchestrator>()));

            return objectContainer;
        }

        public static IObjectContainer InitialiseDatabaseData(this IObjectContainer objectContainer)
        {
            objectContainer.Resolve<ITestTransactionRepository>().InitialiseDatabaseData();

            return objectContainer;
        }

        //public static IObjectContainer SetupEapOrchestrator(this IObjectContainer objectContainer, IContainer container)
        //{

        //    objectContainer.RegisterInstanceAs(new EmployerAccountPayeOrchestrator(container.GetInstance<IMediator>(),
        //        container.GetInstance<ILog>(), container.GetInstance<ICookieStorageService<EmployerAccountData>>()
        //        , container.GetInstance<EmployerApprenticeshipsServiceConfiguration>()));

        //    return objectContainer;
        //}

        //public static IObjectContainer SetupEapController(this IObjectContainer objectContainer, IContainer container)
        //{
        //    objectContainer.RegisterInstanceAs(new EmployerAccountPayeController(container.GetInstance<IAuthenticationService>(),
        //        objectContainer.Resolve<EmployerAccountPayeOrchestrator>(),
        //        container.GetInstance<IAuthorizationService>(),
        //        container.GetInstance<IMultiVariantTestingService>(),
        //        container.GetInstance<ICookieStorageService<FlashMessageViewModel>>()));
        //    return objectContainer;
        //}
    }
}
