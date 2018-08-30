using System.Data.Common;
using System.Threading.Tasks;
using BoDi;
using NServiceBus;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.EmployerAccounts.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Steps
{
    [Binding]
    public sealed class Hooks
    {
        private readonly ObjectContext _objectContext;
        private readonly IObjectContainer _objectContainer;
        private readonly IContainer _container;

        private IEndpointInstance _initiateJobServiceBusEndpoint;

        public Hooks(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContext = objectContext;
            _objectContainer = objectContainer;
            
            _container = new Container(c => { c.AddRegistry<EmployerAccountsAcceptanceTestsRegistry>(); });
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _container
                .InitialiseAndUseInfrastructureDbContexts()
                .InitialiseAndUseEmployerAccountsDbContexts();

            _objectContainer
                .AddImplementationsRequiredInCommonSteps(_container)
                .AddEmployerAccountsImplementations(_container)
                .SetupEatOrchestrator(_container)
                .SetupEatController(_container)
                .SetupEapOrchestrator(_container)
                .SetupEapController(_container);

            await StartNServiceBusEndPoints();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _container.FinalizeTransactions();

            StopNServiceBusEndPoints();
            DisposeContainers();
        }

        private void StopNServiceBusEndPoints()
        {
            StopInitiateJobServiceBusEndpoint();
        }

        private void DisposeContainers()
        {
            _objectContainer.Dispose();
            _container.Dispose();
        }

        private async Task StartNServiceBusEndPoints()
        {
            await StartInitiateJobServiceBusEndpoint();

            _container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_initiateJobServiceBusEndpoint);
            });
        }

        private async Task StartInitiateJobServiceBusEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.AcceptanceTests.Steps.Jobs")
                .UseAzureServiceBusTransport(() => _container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .UseLicense(_container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
                .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseSendOnly()
                .UseStructureMapBuilder(_container);

            _initiateJobServiceBusEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _objectContext.InitiateJobServiceBusEndpoint = _initiateJobServiceBusEndpoint;
        }

        private void StopInitiateJobServiceBusEndpoint()
        {
            _initiateJobServiceBusEndpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}
