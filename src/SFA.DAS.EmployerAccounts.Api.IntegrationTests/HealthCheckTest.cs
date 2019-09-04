using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using FluentAssertions;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using StructureMap;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class HealthCheckTest
    {
        private HttpServer _server;
        private IEndpointInstance _endpoint;

        [SetUp]
        public void Startup()
        {
            string test = ConfigurationManager.AppSettings["EnvironmentName"];

            var config = new HttpConfiguration();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            WebApiConfig.Register(config);


            _server = new HttpServer(config);

            var container = config.DependencyResolver.GetService(typeof(IContainer)) as IContainer;

            StartServiceBusEndpoint(container);

            Debug.WriteLine(container.WhatDidIScan());
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

            StopServiceBusEndpoint();
        }

        private void StartServiceBusEndpoint(IContainer container)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.Api")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerAccountsConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(container.GetInstance<EmployerAccountsConfiguration>().NServiceBusLicense))
                .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(container)
                .UseUnitOfWork();

            _endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });
        }

        private void StopServiceBusEndpoint()
        {
            _endpoint?.Stop().GetAwaiter().GetResult();
        }


    }
}
