using System;
using System.Data.Common;
using System.Threading.Tasks;
using BoDi;
using NServiceBus;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Extensions;
using StructureMap;
using TechTalk.SpecFlow;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.UnitOfWork;
using SFA.DAS.UnitOfWork.NServiceBus;
using IoC = SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution.IoC;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class Hooks
    {
        private static IContainer _container;
        private static IEndpointInstance _initiateJobServiceBusEndpoint;
        private static IContainer _nestedContainer;
        private readonly IObjectContainer _objectContainer;
        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            _container = IoC.Initialize();

            await  StartNServiceBusEndPoints();
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _nestedContainer = _container.GetNestedContainer();

            _objectContainer
                .AddRequiredImplementations(_nestedContainer);

            await ResolveIUnitOfWorkManager().BeginAsync();
        }

        private IUnitOfWorkManager ResolveIUnitOfWorkManager()
        {
            return _objectContainer.Resolve<IUnitOfWorkManager>();
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            try
            {
                await ResolveIUnitOfWorkManager().EndAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _nestedContainer.Dispose();
            _objectContainer.Dispose();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            StopNServiceBusEndPoints();
            _container.Dispose();
        }

        private static async Task StartNServiceBusEndPoints()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.AcceptanceTests.Steps.Jobs")
                .UseAzureServiceBusTransport(() => _container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(_container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(_container)
                .UseUnitOfWork();

            endpointConfiguration.PurgeOnStartup(true);

            _initiateJobServiceBusEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _initiateJobServiceBusEndpoint
                .UseEndpoint(_container);
        }

        private static void StopNServiceBusEndPoints()
        {
            _initiateJobServiceBusEndpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}