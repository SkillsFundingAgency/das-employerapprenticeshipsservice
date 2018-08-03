using Microsoft.ApplicationInsights;
using NLog;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Web.Logging;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Logging;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.EntityFramework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.NServiceBus.Mvc;
using SFA.DAS.NServiceBus.NewtonsoftSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using SFA.DAS.Web.Policy;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ConfigurationHelper = SFA.DAS.EmployerAccounts.Configuration.ConfigurationHelper;
using Environment = SFA.DAS.EmployerAccounts.Configuration.Environment;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private IEndpointInstance _endpoint;

        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            LoggingConfig.ConfigureLogging();
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            if (ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local, Environment.At, Environment.Test))
            {
                SystemDetailsViewModel.EnvironmentName = ConfigurationHelper.CurrentEnvironmentName;
                SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            StartServiceBusEndpoint();
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(new List<IHttpContextPolicy> { new ResponseHeaderRestrictionPolicy() })
                .Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }

        protected void Application_End()
        {
            StopServiceBusEndpoint();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (exception is HttpException httpException && httpException.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                return;
            }

            Dictionary<string, object> properties = null;

            try
            {
                properties = new Dictionary<string, object>
                {
                    ["HttpMethod"] = Request.HttpMethod,
                    ["IsAuthenticated"] = Request.IsAuthenticated,
                    ["Url"] = Request.Url.PathAndQuery,
                    ["UrlReferrer"] = Request.UrlReferrer?.PathAndQuery
                };
            }
            catch (Exception)
            {
                // Request not available
            }

            var message = exception.GetMessage();
            var telemetryClient = new TelemetryClient();

            Logger.Error(exception, message, properties);
            telemetryClient.TrackException(exception);
        }

        private void StartServiceBusEndpoint()
        {
            var container = StructuremapMvc.StructureMapDependencyScope.Container;

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EmployerAccounts.Web")
                .SetupAzureServiceBusTransport(() => container.GetInstance<EmployerAccountsConfiguration>().ServiceBusConnectionString)
                .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>()
                .SetupErrorQueue()
                .SetupInstallers()
                .SetupLicense(container.GetInstance<EmployerAccountsConfiguration>().NServiceBusLicense)
                .SetupMsSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .SetupNewtonsoftSerializer()
                .SetupNLogFactory()
                .SetupOutbox(GlobalFilters.Filters)
                .SetupStructureMapBuilder(container);

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
