using System.Web;
using System.Web.Http;
using NServiceBus;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerAccounts.Api
{
    public class WebApiApplication : HttpApplication
    {
        private IEndpointInstance _endpoint;
        private ServiceBusEndPointConfigureAndRun _serviceBusEndpointManager;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            _serviceBusEndpointManager = 
                new ServiceBusEndPointConfigureAndRun(
                GlobalConfiguration.Configuration.DependencyResolver.GetService<IContainer>());

            _serviceBusEndpointManager
                .ConfigureAndStartServiceBusEndpoint();
        }

        protected void Application_End()
        {
            _serviceBusEndpointManager?
                .StopServiceBusEndpoint();
        }
    }
}