using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerApprenticeshipsService.Web.Startup))]

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                LoginPath = new PathString("/home/FakeUserSignIn")
            });
        }
    }
}
