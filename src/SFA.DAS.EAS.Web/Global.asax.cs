using System;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using NLog;
using NLog.Targets;
using SFA.DAS.EAS.Infrastructure.Logging;

namespace SFA.DAS.EAS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static RedisTarget _redisTarget; // Required to ensure assembly is copied to output.

        protected void Application_Start()
        {
            LoggingConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            FluentValidationModelValidatorProvider.Configure();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            application?.Context?.Response.Headers.Remove("Server");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            _logger.Error(exception);

            var tc = new TelemetryClient();
            tc.TrackTrace($"{exception.Message} - {exception.InnerException}");
        }
    }
}
