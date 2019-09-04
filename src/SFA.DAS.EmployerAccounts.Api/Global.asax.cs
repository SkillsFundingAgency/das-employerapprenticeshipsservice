using System.Data.Common;
using System.Net;
using System.Web;
using System.Web.Http;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerAccounts.Api
{
    public class WebApiApplication : HttpApplication
    {
        private IEndpointInstance _endpoint;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            _endpoint = StartServiceBusEndpoint(GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>());
        }

        protected void Application_End()
        {
            StopServiceBusEndpoint(_endpoint);
        }

        public static IEndpointInstance StartServiceBusEndpoint(IContainer container)
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

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            container.Configure(c =>
            {
                c.For<IMessageSession>().Use(endpoint);
            });

            return endpoint;
        }

        public static void StopServiceBusEndpoint(IEndpointInstance endpoint)
        {
            endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}