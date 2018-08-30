using AutoMapper;
using BoDi;
using MediatR;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions
{
    public static class ObjectContainerExtensions
    {
        public static IObjectContainer AddEmployerAccountsImplementations(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<Data.EmployerAccountsDbContext>());

            objectContainer.RegisterInstanceAs(container.GetInstance<EAS.Domain.Data.IPayeRepository>());

            return objectContainer;
        }

        public static IObjectContainer AddImplementationsRequiredInCommonSteps(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<EAS.Infrastructure.Data.EmployerAccountsDbContext>());
            objectContainer.RegisterInstanceAs(container.GetInstance<EAS.Infrastructure.Data.EmployerFinanceDbContext>());

            objectContainer.RegisterInstanceAs(container.GetInstance<IHashingService>());
            objectContainer.RegisterInstanceAs(container.GetInstance<IMediator>());
            objectContainer.RegisterInstanceAs(container.GetInstance<IPublicHashingService>());
            objectContainer.RegisterInstanceAs(container.GetInstance<ILog>());

            objectContainer.RegisterInstanceAs(new Mock<IEventsApi>());
            objectContainer.RegisterInstanceAs(new Mock<IAuthenticationService>());
            objectContainer.RegisterInstanceAs(new Mock<ICurrentDateTime>());
            objectContainer.RegisterInstanceAs(new Mock<EmployerFinance.Interfaces.ICurrentDateTime>());
            objectContainer.RegisterInstanceAs(new Mock<ICookieStorageService<FlashMessageViewModel>>());
            objectContainer.RegisterInstanceAs(new Mock<ICookieStorageService<EmployerAccountData>>());
            objectContainer.RegisterInstanceAs(new Mock<IAuthorizationService>());

            container.Configure(c =>
            {
                c.For<IAuthenticationService>().Use(objectContainer.Resolve<Mock<IAuthenticationService>>().Object);
                c.For<IAuthorizationService>().Use(objectContainer.Resolve<Mock<IAuthorizationService>>().Object);
                c.For<ICookieStorageService<EmployerAccountData>>().Use(objectContainer.Resolve<Mock<ICookieStorageService<EmployerAccountData>>>().Object);
                c.For<ICookieStorageService<FlashMessageViewModel>>().Use(objectContainer.Resolve<Mock<ICookieStorageService<FlashMessageViewModel>>>().Object);
                c.For<ICurrentDateTime>().Use(objectContainer.Resolve<Mock<ICurrentDateTime>>().Object);
                c.For<EmployerFinance.Interfaces.ICurrentDateTime>().Use(objectContainer.Resolve<Mock<EmployerFinance.Interfaces.ICurrentDateTime>>().Object);
                c.For<IEventsApi>().Use(objectContainer.Resolve<Mock<IEventsApi>>().Object);
            });

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
            objectContainer.RegisterInstanceAs(new EmployerAccountTransactionsController(container.GetInstance<IAuthenticationService>(),
                container.GetInstance<IAuthorizationService>(),
                container.GetInstance<IHashingService>(),
                container.GetInstance<IMediator>(),
                objectContainer.Resolve<EmployerAccountTransactionsOrchestrator>(),
                container.GetInstance<IMultiVariantTestingService>(),
                container.GetInstance<ICookieStorageService<FlashMessageViewModel>>(),
                container.GetInstance<ITransactionFormatterFactory>(),
                container.GetInstance<IMapper>()));

            return objectContainer;
        }

        public static IObjectContainer SetupEapOrchestrator(this IObjectContainer objectContainer, IContainer container)
        {

            objectContainer.RegisterInstanceAs(new EmployerAccountPayeOrchestrator(container.GetInstance<IMediator>(),
                container.GetInstance<ILog>(), container.GetInstance<ICookieStorageService<EmployerAccountData>>()
                , container.GetInstance<EmployerApprenticeshipsServiceConfiguration>()));

            return objectContainer;
        }

        public static IObjectContainer SetupEapController(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstanceAs(new EmployerAccountPayeController(container.GetInstance<IAuthenticationService>(),
                objectContainer.Resolve<EmployerAccountPayeOrchestrator>(),
                container.GetInstance<IAuthorizationService>(),
                container.GetInstance<IMultiVariantTestingService>(),
                container.GetInstance<ICookieStorageService<FlashMessageViewModel>>()));
            return objectContainer;
        }
    }
}
