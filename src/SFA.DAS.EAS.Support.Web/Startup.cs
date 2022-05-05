using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.SiteConnection;

[assembly: OwinStartup(typeof(SFA.DAS.EAS.Support.Web.Startup))]
namespace SFA.DAS.EAS.Support.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var ioc = DependencyResolver.Current;
            var logger = ioc.GetService<ILog>();
            logger.Info("Starting Web Role");
            var siteConnectorSettings = ioc.GetService<ISiteValidatorSettings>();

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Tenant = siteConnectorSettings.Tenant,
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidAudiences = siteConnectorSettings.Audience.Split(',')
                }
            });

            logger.Info("Web role started");
        }
    }
}