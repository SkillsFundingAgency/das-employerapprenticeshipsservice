using Microsoft.Owin;
using Owin;
using SFA.DAS.EmployerFinance.Api;

[assembly: OwinStartup(typeof(Startup))]

namespace SFA.DAS.EmployerFinance.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
