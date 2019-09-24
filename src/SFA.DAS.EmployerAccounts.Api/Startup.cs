using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SFA.DAS.AutoConfiguration;
using System.Web.Http;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerAccounts.Api.Startup))]

namespace SFA.DAS.EmployerAccounts.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
            IEnvironmentService environmentService = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IEnvironmentService)) as IEnvironmentService;

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Tenant = environmentService.GetVariable("idaTenant"),
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidAudience = environmentService.GetVariable("idaAudience")
                }
            });
        }
    }
}
