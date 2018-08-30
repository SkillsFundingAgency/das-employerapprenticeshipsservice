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
using SFA.DAS.UnitOfWork.NServiceBus;
using IoC = SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution.IoC;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class Hooks
    {
        private static readonly IContainer Container;

        private readonly ObjectContext _objectContext;
        private readonly IObjectContainer _objectContainer;

        private IEndpointInstance _jobsServiceBusEndpoint;

        private IEndpointInstance _initiateJobServiceBusEndpoint;

        static Hooks()
        {
            Container =  IoC.Initialize();

            Container
                .InitialiseAndUseEmployerFinanceDbContexts();
        }

        public Hooks(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContext = objectContext;
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _objectContainer
                .AddImplementationsRequiredInCommonSteps(Container)
                .AddEmployerFinanceImplementations(Container)
                .SetupEatOrchestrator(Container)
                .SetupEatController(Container)
                .InitialiseDatabaseData();

            await StartNServiceBusEndPoints();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            StopNServiceBusEndPoints();
            DisposeContainers();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Container.FinalizeTransactions();
            Container.Dispose();
        }

        private void StopNServiceBusEndPoints()
        {
            StopFinanceJobsEndPoint();
            StopInitiateJobServiceBusEndpoint();
        }

        private void DisposeContainers()
        {
            _objectContainer.Dispose();
        }

        private async Task StartNServiceBusEndPoints()
        {
            await StartFinanceJobsEndPoint();
            await StartInitiateJobServiceBusEndpoint();
        }

        private async Task StartFinanceJobsEndPoint()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.AcceptanceTests.Steps.MessageHandlers")
                .UseAzureServiceBusTransport(() => Container.GetInstance<EmployerAccountsConfiguration>().ServiceBusConnectionString)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(Container.GetInstance<EmployerAccountsConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseSqlServerPersistence(() => Container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(Container)
                .UseUnitOfWork();

            endpointConfiguration.PurgeOnStartup(true);

            _jobsServiceBusEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _objectContext.FinanceJobsServiceBusEndpoint = _jobsServiceBusEndpoint;
        }

        private void StopFinanceJobsEndPoint()
        {
            _jobsServiceBusEndpoint?.Stop().GetAwaiter().GetResult();
        }

        private async Task StartInitiateJobServiceBusEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.AcceptanceTests.Steps.Jobs")
                .UseAzureServiceBusTransport(() => Container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(Container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseSqlServerPersistence(() => Container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(Container)
                .UseUnitOfWork();

            _initiateJobServiceBusEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _objectContext.InitiateJobServiceBusEndpoint = _initiateJobServiceBusEndpoint;
       }

        private void StopInitiateJobServiceBusEndpoint()
        {
            _initiateJobServiceBusEndpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}