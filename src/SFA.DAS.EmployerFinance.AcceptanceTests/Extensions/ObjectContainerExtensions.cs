using System;
using System.Threading;
using System.Threading.Tasks;
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
        public static void RegisterInstances(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstance<EmployerAccountTransactionsOrchestrator>(container);
            objectContainer.RegisterInstance<EmployerAccountTransactionsController>(container);
            objectContainer.RegisterInstance<IContainer>(container);
            objectContainer.RegisterInstance<IHashingService>(container);
            objectContainer.RegisterInstance<ILog>(container);
            objectContainer.RegisterInstance<IMediator>(container);
            objectContainer.RegisterInstance<IMessageSession>(container);
            objectContainer.RegisterInstance<ITestTransactionRepository>(container);
            objectContainer.RegisterInstance<ITransactionRepository>(container);
        }

        public static void RegisterMocks(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterMock<IApprenticeshipLevyApiClient>(container);
            objectContainer.RegisterMock<IAuthenticationService>(container);
            objectContainer.RegisterMock<IAuthorizationService>(container);
            objectContainer.RegisterMock<ICurrentDateTime>(container);
            objectContainer.RegisterMock<IEmployerAccountRepository>(container);
            objectContainer.RegisterMock<IEventsApi>(container);
            objectContainer.RegisterMock<IMembershipRepository>(container);
            objectContainer.RegisterMock<IPayeRepository>(container);
        }

        public static async Task ScopeAsync(this IObjectContainer objectContainer, Func<IObjectContainer, Task> step)
        {
            using (var nestedContainer = objectContainer.Resolve<IContainer>().GetNestedContainer())
            using (var nestedObjectContainer = new ObjectContainer(objectContainer))
            {
                var unitOfWorkManager = nestedContainer.GetInstance<IUnitOfWorkManager>();
                
                nestedObjectContainer.RegisterInstances(nestedContainer);

                await unitOfWorkManager.BeginAsync().ConfigureAwait(false);

                try
                {
                    await step(nestedObjectContainer);
                }
                catch (Exception ex)
                {
                    await unitOfWorkManager.EndAsync(ex).ConfigureAwait(false);
                    throw;
                }

                await unitOfWorkManager.EndAsync().ConfigureAwait(false);
            }
        }

        public static async Task RunStepsInIsolation(this IObjectContainer container, CancellationToken cancellationToken, params Func<IObjectContainer, Task>[] funcs)

        {
            foreach (var func in funcs)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await container.ScopeAsync(func);
            }
        }

        private static void RegisterInstance<T>(this IObjectContainer objectContainer, IContainer container) where T : class
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<T>());
        }

        private static void RegisterMock<T>(this IObjectContainer objectContainer, IContainer container) where T : class
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<Mock<T>>());
        }
    }
}
