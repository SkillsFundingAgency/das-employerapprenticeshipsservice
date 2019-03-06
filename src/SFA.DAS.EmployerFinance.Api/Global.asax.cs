using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using NServiceBus;
using SFA.DAS.EmployerFinance.Startup;
using DependencyResolver = System.Web.Mvc.DependencyResolver;

namespace SFA.DAS.EmployerFinance.Api
{
    public class WebApiApplication : HttpApplication
    {
        private IEndpointInstance _endpoint;

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
