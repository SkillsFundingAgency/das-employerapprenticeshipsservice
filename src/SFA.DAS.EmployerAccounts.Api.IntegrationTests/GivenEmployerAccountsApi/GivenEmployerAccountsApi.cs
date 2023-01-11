using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi
{
    [ExcludeFromCodeCoverage]
    public abstract class GivenEmployerAccountsApi
    {
        private HttpServer _server;
        private ServiceBusEndPointConfigureAndRun _serviceBusEndpointManager;
        protected HttpResponseMessage Response;
        private IContainer _container;

        [SetUp]
        public void Startup()
        {
            var config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            WebApiConfig.Register(config);

            _container = config.DependencyResolver.GetService(typeof(IContainer)) as IContainer;

            _serviceBusEndpointManager = new ServiceBusEndPointConfigureAndRun(_container);

            _serviceBusEndpointManager.ConfigureAndStartServiceBusEndpoint();

            _server = new HttpServer(config);
        }

        protected void WhenControllerActionIsCalled(string uri)
        {
            using (var client = new HttpClient(_server))
            {
                Response = client.GetAsync(uri).Result;
            }
        }

        protected async Task InitialiseEmployerAccountData(Func<EmployerAccountsDbBuilder, Task> initialiseAction)
        {
            var builder = new EmployerAccountsDbBuilder(_container.GetNestedContainer());
            
            builder.BeginTransaction();
            try
            {
                await initialiseAction(builder);
                builder.CommitTransaction();
            }
            catch (Exception)
            {
                builder.RollbackTransaction();
                throw;
            }
        }

        [TearDown]
        public void Cleanup()
        {
            _serviceBusEndpointManager?.StopServiceBusEndpoint();
        }
    }
}