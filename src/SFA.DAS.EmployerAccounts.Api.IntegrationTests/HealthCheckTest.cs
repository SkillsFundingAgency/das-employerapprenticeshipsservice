using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using FluentAssertions;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class HealthCheckTest
    {
        private HttpServer _server;

        [SetUp]
        public void Startup()
        {
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);

            _server = new HttpServer(config);
        }

        [Test]
        public async Task HealthCheckReturnsOK()
        {
            var client = new HttpClient(_server);

            var response = await client.GetAsync(@"https://localhost:44330/api/healthcheck");

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
        }
    }
}
