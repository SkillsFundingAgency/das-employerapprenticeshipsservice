using System.Web;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Logging;

namespace SFA.DAS.EmployerFinance.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoggingConfig.ConfigureLogging();
            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");
        }

        protected void Application_End()
        {
        }

    }
}
