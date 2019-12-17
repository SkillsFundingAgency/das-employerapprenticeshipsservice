using System.Configuration;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Infrastructure.Logging;
using Microsoft.ApplicationInsights.Extensibility;


namespace SFA.DAS.EAS.Account.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoggingConfig.ConfigureLogging();
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
        }

        protected void Application_End()
        {
        }
    }
}
