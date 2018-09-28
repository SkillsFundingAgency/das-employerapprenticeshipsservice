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

        private IEndpointInstance _initiateJobServiceBusEndpoint;

        static Hooks()
        {
            Container =  IoC.Initialize();
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
            Container.Dispose();
        }

        private void StopNServiceBusEndPoints()
        {
            StopInitiateJobServiceBusEndpoint();
        }

        private void DisposeContainers()
        {
            _objectContainer.Dispose();
        }

        private async Task StartNServiceBusEndPoints()
        {
            await StartInitiateJobServiceBusEndpoint();
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
            Container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_initiateJobServiceBusEndpoint);
            });
       }

        private void StopInitiateJobServiceBusEndpoint()
        {
            _initiateJobServiceBusEndpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}