﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using NLog;
using Owin;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.OidcMiddleware;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerApprenticeshipsService.Web.Startup))]

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class Startup
    {
      
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";


        public void Configuration(IAppBuilder app)
        {
            var config = GetConfigurationObject();

            if (config.Identity.UseFake)
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "Cookies",
                    LoginPath = new PathString("/home/FakeUserSignIn")
                });
            }
            else
            {
                var authenticationOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AuthenticationOrchestraor>();
                var logger = LogManager.GetLogger("Startup");
            


                JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "Cookies",
                    ExpireTimeSpan = new TimeSpan(0,10,0),
                    SlidingExpiration = true
                });

                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "TempState",
                    AuthenticationMode = AuthenticationMode.Passive
                });

                var constants = new Constants(config.Identity.BaseAddress);
                app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
                {
                    ClientId = config.Identity.ClientId, 
                    ClientSecret = config.Identity.ClientSecret, 
                    Scopes = "openid",
                    BaseUrl = constants.BaseAddress,
                    TokenEndpoint = constants.TokenEndpoint(),
                    UserInfoEndpoint = constants.UserInfoEndpoint(),
                    AuthorizeEndpoint = constants.AuthorizeEndpoint(),
                    AuthenticatedCallback = identity =>
                    {
                        PostAuthentiationAction(identity, authenticationOrchestrator, logger);
                    }
                });
            }
        }

        private static void PostAuthentiationAction(ClaimsIdentity identity, AuthenticationOrchestraor authenticationOrchestrator, ILogger logger)
        {
            logger.Info("PostAuthenticationAction called");
            var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == @"sub")?.Value;
            var email = identity.Claims.FirstOrDefault(claim => claim.Type == @"email")?.Value;
            var firstName = identity.Claims.FirstOrDefault(claim => claim.Type == @"given_name")?.Value;
            var lastName = identity.Claims.FirstOrDefault(claim => claim.Type == @"family_name")?.Value;
            logger.Info("Claims retrieved from OIDC server {0}: {1} : {2} : {3}", userRef, email,  firstName,  lastName);

            Task.Run(async ()  =>
            {
                await authenticationOrchestrator.SaveIdentityAttributes(userRef, email, firstName, lastName);
            }).Wait();
           
            //HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Path, true);
            
        }

        private static EmployerApprenticeshipsServiceConfiguration GetConfigurationObject()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(
               configurationRepository,
               new ConfigurationOptions(ServiceName, environment, "1.0"));

            var config = configurationService.Get<EmployerApprenticeshipsServiceConfiguration>();

            return config;
        }


        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository =
                    new AzureTableStorageConfigurationRepository(
                        CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }


    public class Constants
    {

        public Constants(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }
        public string BaseAddress { get; set; }

        public string AuthorizeEndpoint() => BaseAddress + "/Login/dialog/appl/oidc/wflow/authorize";
        public string LogoutEndpoint() => BaseAddress + "/connect/endsession";
        public string TokenEndpoint() =>  BaseAddress + "/Login/rest/appl/oidc/wflow/token";
        public string UserInfoEndpoint() =>  BaseAddress + "/Login/rest/appl/oidc/wflow/userinfo";
        public string IdentityTokenValidationEndpoint() =>  BaseAddress + "/connect/identitytokenvalidation";
        public string TokenRevocationEndpoint() => BaseAddress + "/connect/revocation";

        public string ChangePasswordLink() => BaseAddress + "/Login/dialog/appl/selfcare/wflow/changepwdpg";

        public string ChangeEmailLink() => BaseAddress + "/Login/dialog/appl/selfcare/wflow/changeemailpg";
    }
}
