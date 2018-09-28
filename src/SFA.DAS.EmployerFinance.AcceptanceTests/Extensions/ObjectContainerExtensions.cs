using System.Collections.Generic;
using System.Data.Common;
using BoDi;
using HMRC.ESFA.Levy.Api.Client;
using MediatR;
using Moq;
using NServiceBus;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.UnitOfWork;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class ObjectContainerExtensions
    {
        public static IObjectContainer AddRequiredImplementations(this IObjectContainer objectContainer, IContainer nestedContainer)
        {
            //objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<DbConnection>());
            //objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<EmployerAccountsDbContext>());
            //objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<EmployerFinanceDbContext>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<IEndpointInstance>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<IEnumerable<IUnitOfWorkManager>>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<IHashingService>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<IMediator>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<ILog>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<ITestTransactionRepository>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<ITransactionRepository>());
            objectContainer.RegisterInstanceAs(nestedContainer.GetInstance<IUnitOfWorkManager>());

            objectContainer.RegisterInstanceAs(new Mock<IApprenticeshipLevyApiClient>());
            objectContainer.RegisterInstanceAs(new Mock<IAuthenticationService>());
            objectContainer.RegisterInstanceAs(new Mock<IAuthorizationService>());
            objectContainer.RegisterInstanceAs(new Mock<ICurrentDateTime>());
            objectContainer.RegisterInstanceAs(new Mock<IEmployerAccountRepository>());
            objectContainer.RegisterInstanceAs(new Mock<IEventsApi>());
            objectContainer.RegisterInstanceAs(new Mock<IMembershipRepository>());
            objectContainer.RegisterInstanceAs(new Mock<IPayeRepository>());

            nestedContainer.Configure(c =>
            {
                c.For<IApprenticeshipLevyApiClient>().Use(objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>().Object);
                c.For<IAuthenticationService>().Use(objectContainer.Resolve<Mock<IAuthenticationService>>().Object);
                c.For<IAuthorizationService>().Use(objectContainer.Resolve<Mock<IAuthorizationService>>().Object);
                c.For<ICurrentDateTime>().Use(objectContainer.Resolve<Mock<ICurrentDateTime>>().Object);
                c.For<IEmployerAccountRepository>().Use(objectContainer.Resolve<Mock<IEmployerAccountRepository>>().Object);
                c.For<IEventsApi>().Use(objectContainer.Resolve<Mock<IEventsApi>>().Object);
                c.For<IMembershipRepository>().Use(objectContainer.Resolve<Mock<IMembershipRepository>>().Object);
                c.For<IPayeRepository>().Use(objectContainer.Resolve<Mock<IPayeRepository>>().Object);
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
            objectContainer.RegisterInstanceAs(new EmployerAccountTransactionsController(
                container.GetInstance<IAuthenticationService>(),
                container.GetInstance<EmployerAccountTransactionsOrchestrator>()));

            return objectContainer;
        }
    }
}
