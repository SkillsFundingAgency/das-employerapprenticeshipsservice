using System.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SFA.DAS.EmployerFinance.Api;

[assembly: OwinStartup(typeof(Startup))]

namespace SFA.DAS.EmployerFinance.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            _ = app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Tenant = ConfigurationManager.AppSettings["idaTenant"],
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidAudiences = ConfigurationManager.AppSettings["FinanceApiIdaAudience"].ToString().Split(',')
                }
            });
        }
    }
}
