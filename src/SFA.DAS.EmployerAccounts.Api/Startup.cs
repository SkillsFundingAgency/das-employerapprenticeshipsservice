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
            //app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            //{
            //    Tenant = CloudConfigurationManager.GetSetting("idaTenant"),
            //    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
            //    {
            //        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            //        ValidAudience = CloudConfigurationManager.GetSetting("idaAudience")
            //    }
            //});
        }
    }
}
