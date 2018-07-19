using System;
using System.Data.Common;
using AutoMapper;
using BoDi;
using MediatR;
using NServiceBus;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.TestCommon.Steps;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.EntityFramework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.NServiceBus.NewtonsoftSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Steps
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private readonly IContainer _container;

        private IEndpointInstance _endpoint;

        public Hooks(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;

            _container = new Container(c =>
            {
                c.For<IEventsApi>().Use(objectContext.EventsApiMock.Object);

                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<RepositoriesRegistry>();

                c.AddRegistry<EmployerAccountsDbContextRegistry>();
                c.AddRegistry<EmployerFinanceDbContextRegistry>();

                c.AddRegistry<NServiceBusRegistry>();
                c.For<IEventPublisher>().Use<EventPublisher>();

                c.Scan(scanner =>
                {
                    scanner.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                    scanner.RegisterConcreteTypesAgainstTheFirstInterface();
                });
            });

            StartServiceBusEndpoint();

            _container.Configure(c =>
            {
                c.For<EmployerAccountsDbContext>().Use(_container.GetInstance<EmployerAccountsDbContext>());
            });
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _container.GetInstance<EmployerAccountsDbContext>().Database.BeginTransaction();

            _objectContainer.RegisterInstanceAs(_container);
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IEventsApi>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IEventsApiClientConfiguration>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IHashingService>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IMapper>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IMediator>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IPayeRepository>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IPublicHashingService>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<ILog>());

            _objectContainer.RegisterInstanceAs(_container.GetInstance<IMultiVariantTestingService>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IEventPublisher>());

            _objectContainer.RegisterInstanceAs(_container.GetInstance<EmployerAccountsDbContext>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<EmployerFinanceDbContext>());
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _container.GetInstance<EmployerAccountsDbContext>().Database.CurrentTransaction.Rollback();
            // _container.GetInstance<EmployerAccountsDbContext>().Database.CurrentTransaction.Commit();
            StopServiceBusEndpoint();

            _objectContainer.Dispose();
            _container.Dispose();
        }

        private void StartServiceBusEndpoint()
        {

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
                .SetupAzureServiceBusTransport(_container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().ServiceBusConnectionString)
                .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>()
                .SetupErrorQueue()
                .SetupHeartbeat()
                .SetupInstallers()
                .SetupMetrics()
                .SetupMsSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                .SetupNewtonsoftSerializer()
                .SetupNLogFactory()
                .SetupOutbox()
                .SetupStructureMapBuilder(_container);

            _endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            _container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });
        }

        private void StopServiceBusEndpoint()
        {
            _endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}