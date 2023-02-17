//using System.Web.Mvc;
//using Microsoft.Owin;
//using Microsoft.Owin.Security.ActiveDirectory;
//using Owin;
//using SFA.DAS.NLog.Logger;
//using SFA.DAS.Support.Shared.SiteConnection;

//[assembly: OwinStartup(typeof(SFA.DAS.EAS.Support.Web.Startup))]
//namespace SFA.DAS.EAS.Support.Web
//{
//    public class StartupOld
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            var ioc = DependencyResolver.Current;
//            var logger = ioc.GetService<ILog>();
//            var siteValidatorSettings = ioc.GetService<ISiteValidatorSettings>();

//            logger.Info($"SiteValidator Configuration Tenant : {siteValidatorSettings.Tenant} and Audience : {siteValidatorSettings.Audience} ");

//            app.UseWindowsAzureActiveDirectoryBearerAuthentication(new WindowsAzureActiveDirectoryBearerAuthenticationOptions
//            {
//                Tenant = siteValidatorSettings.Tenant,
//                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
//                {
//                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
//                    ValidAudiences = siteValidatorSettings.Audience.Split(',')
//                }
//            });            
//        }
//    }
//}