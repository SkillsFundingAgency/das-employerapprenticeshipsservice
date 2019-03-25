using System.Web;
using System.Web.Http;
using SFA.DAS.EmployerFinance.Startup;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerFinance.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.DependencyResolver.GetService<IStartup>().StartAsync().GetAwaiter().GetResult();
        }

        protected void Application_End()
        {
            GlobalConfiguration.Configuration.DependencyResolver.GetService<IStartup>().StopAsync().GetAwaiter().GetResult();
        }
    }
}
