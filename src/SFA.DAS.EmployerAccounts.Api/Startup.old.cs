//using Microsoft.Owin;
//using Microsoft.Owin.Security.ActiveDirectory;
//using Owin;
//using System.Configuration;

//[assembly: OwinStartup(typeof(SFA.DAS.EmployerAccounts.Api.Startup))]

//namespace SFA.DAS.EmployerAccounts.Api
//{
//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            _ = app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
//            {
//                Tenant = ConfigurationManager.AppSettings["idaTenant"],
//                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
//                {
//                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
//                    ValidAudiences = ConfigurationManager.AppSettings["AccountsApiIdentifierUri"].ToString().Split(',')
//                }
//            });
//        }
//    }
//}
