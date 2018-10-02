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
            objectContainer.Copy<IEndpointInstance>(nestedContainer);
            objectContainer.Copy<IUnitOfWorkManager>(nestedContainer);
            objectContainer.Copy<IHashingService>(nestedContainer);
            objectContainer.Copy<IMediator>(nestedContainer);
            objectContainer.Copy<ILog>(nestedContainer);
            objectContainer.Copy<ITestTransactionRepository>(nestedContainer);
            objectContainer.Copy<ITransactionRepository>(nestedContainer);
            objectContainer.Copy<IUnitOfWorkManager>(nestedContainer);

            objectContainer.CopyMock<IApprenticeshipLevyApiClient>(nestedContainer);
            objectContainer.CopyMock<IAuthenticationService>(nestedContainer);
            objectContainer.CopyMock<IAuthorizationService>(nestedContainer);
            objectContainer.CopyMock<ICurrentDateTime>(nestedContainer);
            objectContainer.CopyMock<IEmployerAccountRepository>(nestedContainer);
            objectContainer.CopyMock<IEventsApi>(nestedContainer);
            objectContainer.CopyMock<IMembershipRepository>(nestedContainer);
            objectContainer.CopyMock<IPayeRepository>(nestedContainer);

            return objectContainer;
        }
        private static void Copy<T>(this IObjectContainer objectContainer, IContainer container) where T : class
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<T>());
        }

        private static void CopyMock<TMock>(this IObjectContainer objectContainer, IContainer container) where TMock : class
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<IMock<TMock>>());
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
