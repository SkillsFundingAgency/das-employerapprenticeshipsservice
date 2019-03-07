using System.Web;
using System.Web.Http;
using SFA.DAS.EmployerFinance.Startup;

namespace SFA.DAS.EmployerFinance.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ((IStartup)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IStartup))).StartAsync().GetAwaiter().GetResult();
        }

        protected void Application_End()
        {
            ((IStartup)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IStartup))).StopAsync().GetAwaiter().GetResult();
        }
    }
}
