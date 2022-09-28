using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using NLog;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.EAS.Web.App_Start;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Logging;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.Web.Policy;
using StructureMap;

namespace SFA.DAS.EAS.Web
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
            FluentValidationModelValidatorProvider.Configure();
            LoggingConfig.ConfigureLogging();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

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

            StartServiceBusEndpoint(container);
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(new List<IHttpContextPolicy> { new ResponseHeaderRestrictionPolicy() })
                .Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
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

        protected void Application_End()
        {
            StopServiceBusEndpoint();
        }

        private void StartServiceBusEndpoint(IContainer container)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
                .UseAzureServiceBusTransport(() => container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().ServiceBusConnectionString, container)
                .UseErrorQueue("SFA.DAS.EAS.Web-errors")
                .UseInstallers()
                .UseLicense(WebUtility.HtmlDecode(container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().NServiceBusLicense))
                .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseOutbox()
                .UseStructureMapBuilder(container);

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