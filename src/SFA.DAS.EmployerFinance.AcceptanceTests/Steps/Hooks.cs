using System.Data.Common;
using System.Threading.Tasks;
using BoDi;
using NServiceBus;
using SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class Hooks
    {
        private static IContainer _container;
        private static IEndpointInstance _endpoint;
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            _container = IoC.Initialize();

            await StartNServiceBusEndpoint();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _objectContainer.RegisterInstances(_container);
            _objectContainer.RegisterMocks(_container);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _objectContainer.Dispose();
        }

        [AfterTestRun]
        public static async Task AfterTestRun()
        {
            using (_container)
            {
                await StopNServiceBusEndpoint();
            }
        }

        private static async Task StartNServiceBusEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.AcceptanceTests")
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

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            _container.Configure(c => c.For<IMessageSession>().Use(_endpoint));
        }

        private static Task StopNServiceBusEndpoint()
        {
            return _endpoint.Stop();
        }
    }
}