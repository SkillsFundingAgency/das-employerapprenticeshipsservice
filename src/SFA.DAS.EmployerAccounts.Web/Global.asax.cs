using Microsoft.ApplicationInsights;
using NLog;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Web.Logging;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Logging;
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
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using System.Configuration;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Types;
using SFA.DAS.Audit.Client.Web;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using SFA.DAS.EmployerAccounts.Web;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private IEndpointInstance _endpoint;

        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AntiForgeryConfig.RequireSsl = true;
            AreaRegistration.RegisterAllAreas();
            BinderConfig.RegisterBinders(ModelBinders.Binders);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            LoggingConfig.ConfigureLogging();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            WebMessageBuilders.Register();
            WebMessageBuilders.UserIdClaim = DasClaimTypes.Id;
            WebMessageBuilders.UserEmailClaim = DasClaimTypes.Email;

            AuditMessageFactory.RegisterBuilder(m =>
            {
                m.Source = new Source
                {
                    Component = "EmployerAccounts-Web",
                    System = "EmployerAccounts",
                    Version = typeof(MvcApplication).Assembly.GetName().Version.ToString()
                };
            });

            var container = StructuremapMvc.StructureMapDependencyScope.Container;
            var environmentService = container.GetInstance<IEnvironmentService>();

            if (environmentService.IsCurrent(DasEnv.LOCAL))
            {
                SystemDetailsViewModel.EnvironmentName = DasEnv.LOCAL.ToString();
                SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            if (environmentService.IsCurrent(DasEnv.AT))
            {
                SystemDetailsViewModel.EnvironmentName = DasEnv.AT.ToString();
                SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            if (environmentService.IsCurrent(DasEnv.TEST))
            {
                SystemDetailsViewModel.EnvironmentName = DasEnv.TEST.ToString();
                SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            StartServiceBusEndpoint();
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(new List<IHttpContextPolicy> { new ResponseHeaderRestrictionPolicy() })
                .Apply(new HttpContextWrapper(HttpContextHelper.Current), PolicyConcern.HttpResponse);
        }

        protected void Application_End()
        {
            StopServiceBusEndpoint();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (exception is HttpException httpException && httpException.GetHttpCode() == (int) HttpStatusCode.NotFound)
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
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerAccountsConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue("SFA.DAS.EmployerAccounts.Web-errors")
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(container.GetInstance<EmployerAccountsConfiguration>().NServiceBusLicense))
                .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox(true)
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
