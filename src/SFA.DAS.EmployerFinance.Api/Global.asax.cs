﻿using System.Data.Common;
using System.Web;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.ApplicationInsights.Extensibility;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerFinance.Api
{
    public class WebApiApplication : HttpApplication
    {
        private IEndpointInstance _endpoint;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            StartServiceBusEndpoint();
        }

        protected void Application_End()
        {
            StopServiceBusEndpoint();
        }

        private void StartServiceBusEndpoint()
        {
            var container = GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerFinance.Api")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerFinanceConfiguration>().ServiceBusConnectionString)
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(container.GetInstance<EmployerFinanceConfiguration>().NServiceBusLicense.HtmlDecode())
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
