using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using NLog;
using Owin;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.OidcMiddleware;

[assembly: OwinStartup(typeof(Startup))]

namespace SFA.DAS.EAS.Web
{
    public class Startup
    {
      
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";


        public void Configuration(IAppBuilder app)
        {
            var config = GetConfigurationObject();

            
            var authenticationOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AuthenticationOrchestraor>();
            var logger = LogManager.GetLogger("Startup");
            

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = new TimeSpan(0, 10, 0),
                SlidingExpiration = true
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "TempState",
                AuthenticationMode = AuthenticationMode.Passive
            });

            var constants = new Constants(config.Identity);

            var urlHelper = new UrlHelper();

            UserLinksViewModel.ChangePasswordLink = $"{constants.ChangePasswordLink()}{urlHelper.Encode("https://"+ config.DashboardUrl + "/service/password/change")}";
            UserLinksViewModel.ChangeEmailLink = $"{constants.ChangeEmailLink()}{urlHelper.Encode("https://" + config.DashboardUrl + "/service/email/change")}";

            app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
            {
                ClientId = config.Identity.ClientId,
                ClientSecret = config.Identity.ClientSecret,
                Scopes = config.Identity.Scopes,
                BaseUrl = constants.Configuration.BaseAddress,
                TokenEndpoint = constants.TokenEndpoint(),
                UserInfoEndpoint = constants.UserInfoEndpoint(),
                AuthorizeEndpoint = constants.AuthorizeEndpoint(),
                TokenValidationMethod = config.Identity.UseCertificate ? TokenValidationMethod.SigningKey : TokenValidationMethod.BinarySecret,
                TokenSigningCertificateLoader = GetSigningCertificate(config.Identity.UseCertificate),
                AuthenticatedCallback = identity =>
                {
                    PostAuthentiationAction(identity, authenticationOrchestrator, logger, constants);
                }
            });

            ConfigurationFactory.Current = new IdentityServerConfigurationFactory(config);
            
        }

        private static Func<X509Certificate2> GetSigningCertificate(bool useCertificate)
        {
            if (!useCertificate)
            {
                return null;
            }

            return () =>
            {
                var store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                try
                {
                    var thumbprint = CloudConfigurationManager.GetSetting("TokenCertificateThumbprint");
                    var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                    if (certificates.Count < 1)
                    {
                        throw new Exception($"Could not find certificate with thumbprint {thumbprint} in LocalMachine store");
                    }

                    return certificates[0];
                }
                finally
                {
                    store.Close();
                }
            };
        }

        private static void PostAuthentiationAction(ClaimsIdentity identity, AuthenticationOrchestraor authenticationOrchestrator, ILogger logger, Constants constants)
        {
            logger.Info("PostAuthenticationAction called");
            var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;
            var email = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Email())?.Value;
            var firstName = identity.Claims.FirstOrDefault(claim => claim.Type == constants.GivenName())?.Value;
            var lastName = identity.Claims.FirstOrDefault(claim => claim.Type == constants.FamilyName())?.Value;
            logger.Info("Claims retrieved from OIDC server {0}: {1} : {2} : {3}", userRef, email, firstName, lastName);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identity.Claims.First(c => c.Type == constants.Id()).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, identity.Claims.First(c => c.Type == constants.DisplayName()).Value));
            identity.AddClaim(new Claim("sub", identity.Claims.First(c => c.Type == constants.Id()).Value));
            identity.AddClaim(new Claim("email", identity.Claims.First(c => c.Type == constants.Email()).Value));


            Task.Run(async () =>
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
        private readonly string _baseUrl;
        public IdentityServerConfiguration Configuration { get; set; }
        public Constants(IdentityServerConfiguration configuration)
        {
            this.Configuration = configuration;
            _baseUrl = configuration.ClaimIdentifierConfiguration.ClaimsBaseUrl;
        }

        public string AuthorizeEndpoint() => $"{Configuration.BaseAddress}{Configuration.AuthorizeEndPoint}";
        public string LogoutEndpoint() => $"{Configuration.BaseAddress}{Configuration.LogoutEndpoint}";
        public string TokenEndpoint() => $"{Configuration.BaseAddress}{Configuration.TokenEndpoint}";
        public string UserInfoEndpoint() => $"{Configuration.BaseAddress}{Configuration.UserInfoEndpoint}";
        public string ChangePasswordLink() => Configuration.BaseAddress.Replace("/identity", "") + string.Format(Configuration.ChangePasswordLink, Configuration.ClientId);
        public string ChangeEmailLink() => Configuration.BaseAddress.Replace("/identity", "") + string.Format(Configuration.ChangeEmailLink, Configuration.ClientId);
        public string RegisterLink() => Configuration.BaseAddress.Replace("/identity", "") + string.Format(Configuration.RegisterLink,Configuration.ClientId);
        

        public string Id () => _baseUrl + Configuration.ClaimIdentifierConfiguration.Id;
        public string Email() => _baseUrl + Configuration.ClaimIdentifierConfiguration.Email;
        public string GivenName() => _baseUrl + Configuration.ClaimIdentifierConfiguration.GivenName;
        public string FamilyName() => _baseUrl + Configuration.ClaimIdentifierConfiguration.FaimlyName;
        public string DisplayName() => _baseUrl + Configuration.ClaimIdentifierConfiguration.DisplayName;
        public string RequiresVerification() => _baseUrl + "requires_verification";
    }
}
