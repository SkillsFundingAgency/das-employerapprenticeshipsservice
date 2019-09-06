using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web.Http;
using NServiceBus;
using NUnit.Framework;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi
{
    [ExcludeFromCodeCoverage]
    public abstract class GivenEmployerAccountsApi
    {
        private HttpServer _server;
        private IEndpointInstance _endpoint;
        private IEndpointInstance _nServiceBusEndpoint;
        protected HttpResponseMessage Response;

        [SetUp]
        public void Startup()
        {
            var config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            WebApiConfig.Register(config);

            var container = config.DependencyResolver.GetService(typeof(IContainer)) as IContainer;

            _nServiceBusEndpoint = WebApiApplication.StartServiceBusEndpoint(container);

            _server = new HttpServer(config);
        }

        protected void WhenControllerActionIsCalled(string uri)
        {
            using (var client = new HttpClient(_server))
            {
                Response = client.GetAsync(uri).Result;
            }
        }

        [TearDown]
        public void Cleanup()
        {
            WebApiApplication.StopServiceBusEndpoint(_nServiceBusEndpoint);

            if (_server != null)
                _server.Dispose();
        }
    }
}