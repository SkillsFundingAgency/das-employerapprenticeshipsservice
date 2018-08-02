using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Logging;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.Web.Policy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights;
using NLog;
using SFA.DAS.Logging;
using Environment = SFA.DAS.EmployerFinance.Configuration.Environment;

namespace SFA.DAS.EmployerFinance.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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
    }
}
