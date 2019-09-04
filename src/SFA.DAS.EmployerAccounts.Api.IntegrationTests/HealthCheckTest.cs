using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using FluentAssertions;
using NServiceBus;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class HealthCheckTest
    {
        private HttpServer _server;
        private IEndpointInstance _endpoint;
        private IEndpointInstance _nServiceBusEndpoint;

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

        [Test]
        public async Task HealthCheckReturnsOK()
        {
            var client = new HttpClient(_server);

            var response = await client.GetAsync(@"https://localhost:44330/api/healthcheck");

            client.Dispose();

            Assert
                .IsNotNull(response);

            response
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [TearDown]
        public void Cleanup()
        {
            if (_server != null)
                _server.Dispose();

            WebApiApplication.StopServiceBusEndpoint(_nServiceBusEndpoint);
        }
    }
}
