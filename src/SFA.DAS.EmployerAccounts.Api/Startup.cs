using Microsoft.Owin;
using Owin;
using SFA.DAS.EmployerAccounts.Api;

[assembly: OwinStartup(typeof(Startup))]

namespace SFA.DAS.EmployerAccounts.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
