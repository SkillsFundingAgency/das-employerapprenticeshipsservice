using MediatR;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using NLog;
using Owin;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web;
using SFA.DAS.EmployerFinance.Web.App_Start;
using SFA.DAS.EmployerFinance.Web.Authentication;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.OidcMiddleware;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(Startup))]

namespace SFA.DAS.EmployerFinance.Web
{
    public class Startup
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            var config = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<EmployerFinanceConfiguration>();
            var constants = new Constants(config.Identity);
            var urlHelper = new UrlHelper();

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

            app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
            {
                BaseUrl = config.Identity.BaseAddress,
                ClientId = config.Identity.ClientId,
                ClientSecret = config.Identity.ClientSecret,
                Scopes = config.Identity.Scopes,
                AuthorizeEndpoint = constants.AuthorizeEndpoint(),
                TokenEndpoint = constants.TokenEndpoint(),
                UserInfoEndpoint = constants.UserInfoEndpoint(),
                TokenSigningCertificateLoader = GetSigningCertificate(config.Identity.UseCertificate),
                TokenValidationMethod = config.Identity.UseCertificate ? TokenValidationMethod.SigningKey : TokenValidationMethod.BinarySecret,
                AuthenticatedCallback = identity =>
                {
                    PostAuthentiationAction(identity, constants).GetAwaiter().GetResult();
                }
            });

            ConfigurationFactory.Current = new IdentityServerConfigurationFactory(config);
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
        }

        private static Func<X509Certificate2> GetSigningCertificate(bool useCertificate)
        {
            if (!useCertificate)
            {
                return null;
            }

            return () =>
            {
                var store = new X509Store(StoreLocation.CurrentUser);

                store.Open(OpenFlags.ReadOnly);

                try
                {
                    var thumbprint = ConfigurationManager.AppSettings["TokenCertificateThumbprint"];
                    var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                    if (certificates.Count < 1)
                    {
                        throw new Exception($"Could not find certificate with thumbprint '{thumbprint}' in CurrentUser store.");
                    }

                    return certificates[0];
                }
                finally
                {
                    store.Close();
                }
            };
        }

        private async static Task PostAuthentiationAction(ClaimsIdentity identity, Constants constants)
        {
            var mediator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IMediator>();

            Logger.Info("Retrieving claims from OIDC server.");

            var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;

            Logger.Info($"Retrieved claims from OIDC server for user with external ID '{userRef}'.");

            var id = identity.Claims.First(c => c.Type == constants.Id()).Value;
            var email = identity.Claims.First(c => c.Type == constants.Email()).Value;
            var lastName = identity.Claims.First(c => c.Type == constants.FamilyName()).Value;
            var firstName = identity.Claims.First(c => c.Type == constants.GivenName()).Value;
            var displayName = identity.Claims.First(c => c.Type == constants.DisplayName()).Value;

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim(ClaimTypes.Name, displayName));
            identity.AddClaim(new Claim("sub", id));
            identity.AddClaim(new Claim("email", email));

            await mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                EmailAddress = email,
                UserRef = userRef,
                LastName = lastName,
                FirstName = firstName
            });
        }
    }

    public class Constants
    {
        private readonly string _baseUrl;
        private readonly IdentityServerConfiguration _configuration;

        public Constants(IdentityServerConfiguration configuration)
        {
            _baseUrl = configuration.ClaimIdentifierConfiguration.ClaimsBaseUrl;
            _configuration = configuration;
        }

        public string AuthorizeEndpoint() => $"{_configuration.BaseAddress}{_configuration.AuthorizeEndPoint}";
        public string ChangeEmailLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangeEmailLink, _configuration.ClientId);
        public string ChangePasswordLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangePasswordLink, _configuration.ClientId);
        public string DisplayName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.DisplayName;
        public string Email() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Email;
        public string FamilyName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.FaimlyName;
        public string GivenName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.GivenName;
        public string Id() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Id;
        public string LogoutEndpoint() => $"{_configuration.BaseAddress}{_configuration.LogoutEndpoint}";
        public string TokenEndpoint() => $"{_configuration.BaseAddress}{_configuration.TokenEndpoint}";
        public string UserInfoEndpoint() => $"{_configuration.BaseAddress}{_configuration.UserInfoEndpoint}";
    }
}