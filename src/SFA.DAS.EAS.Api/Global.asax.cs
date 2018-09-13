using System.Data.Common;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Infrastructure.Logging;
using Microsoft.Azure;
using Microsoft.ApplicationInsights.Extensibility;
using NServiceBus;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.Api
{
    public class WebApiApplication : HttpApplication
    {
        private IEndpointInstance _endpoint;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoggingConfig.ConfigureLogging();
            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");

            StartServiceBusEndpoint();
        }

        protected void Application_End()
        {
            StopServiceBusEndpoint();
        }

        private void StartServiceBusEndpoint()
        {
            var container = GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Api")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().ServiceBusConnectionString)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().NServiceBusLicense.HtmlDecode())
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
