﻿using Microsoft.Azure;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerAccounts.Api.Startup))]

namespace SFA.DAS.EmployerAccounts.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Tenant = CloudConfigurationManager.GetSetting("idaTenant"),
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidAudience = CloudConfigurationManager.GetSetting("idaAudience")
                }
            });
        }
    }
}
