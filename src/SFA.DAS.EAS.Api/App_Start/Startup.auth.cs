using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;

namespace SFA.DAS.EAS.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidAudience = CloudConfigurationManager.GetSetting("ida_Audience"),
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                   },
                   Tenant = CloudConfigurationManager.GetSetting("ida_Tenant")
               });
        }
    }
}