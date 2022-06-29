using BoDi;
using Moq;
using NServiceBus;
using SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.Testing.AzureStorageEmulator;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using StructureMap;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
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
            AzureStorageEmulatorManager.StartStorageEmulator();

            _container = IoC.Initialize();

            await StartNServiceBusEndpoint();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _container.GetInstance<ILog>().Info("Starting Scenario.");

            ResetCurrentDateTime(_container);
            ResetFundsExpiryPeriod(_container);

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

        private static void ResetCurrentDateTime(IContainer container)
        {
            var currentDateTime = container.GetInstance<Mock<ICurrentDateTime>>();
            currentDateTime.Setup(x => x.Now).Returns(DateTime.Now);
        }

        private static void ResetFundsExpiryPeriod(IContainer container)
        {
            var config = container.GetInstance<EmployerFinanceConfiguration>();
            config.FundsExpiryPeriod = 24;
        }

        private static async Task StartNServiceBusEndpoint()
        {
            try
            {
                _container.GetInstance<ILog>().Info("Starting endpoint.");

                var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.AcceptanceTests")
                    .UseAzureServiceBusTransport()
                    .UseErrorQueue("SFA.DAS.EmployerFinance.AcceptanceTests-errors")
                    .UseInstallers()
                    .UseLicense(WebUtility.HtmlDecode(_container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense))
                    .UseSqlServerPersistence(() => _container.GetInstance<DbConnection>())
                    .UseNewtonsoftJsonSerializer()
                    .UseNLogFactory()
                    .UseOutbox()
                    .UseStructureMapBuilder(_container)
                    .UseUnitOfWork();

                if (Debugger.IsAttached)
                {
                    endpointConfiguration.PurgeOnStartup(true);
                }

                _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

                _container.Configure(c => c.For<IMessageSession>().Use(_endpoint));

                _container.GetInstance<ILog>().Info("Endpoint Started.");
            }
            catch (Exception e)
            {
                _container.GetInstance<ILog>().Error(e, "Error starting endpoint.");
                throw;
            }
        }

        private static Task StopNServiceBusEndpoint()
        {
            return _endpoint.Stop();
        }
    }
}