using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using Newtonsoft.Json;
using NLog;
using Owin;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.FeatureToggles;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.OidcMiddleware;
using SFA.DAS.OidcMiddleware.Validators;

[assembly: OwinStartup(typeof(Startup))]

namespace SFA.DAS.EmployerAccounts.Web
{
    public class Startup
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private const string AccountDataCookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        public void Configuration(IAppBuilder app)
        {
            var config = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<EmployerAccountsConfiguration>();
            var accountDataCookieStorageService = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ICookieStorageService<EmployerAccountData>>();
            var hashedAccountIdCookieStorageService = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ICookieStorageService<HashedAccountIdModel>>();
            var constants = new Constants(config.Identity);
            var urlHelper = new UrlHelper();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Staff",
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Employer",
                ExpireTimeSpan = new TimeSpan(0, 10, 0),
                SlidingExpiration = true
            });


            // https://skillsfundingagency.atlassian.net/wiki/spaces/ERF/pages/104010807/Staff+IDAMS
            app.UseWsFederationAuthentication(GetADFSOptions());

            app.Map($"/login/staff", conf =>
            {
                conf.Run(context =>
                {
                    // for first iteration of this work, allow deep linking from the support console to the teams view
                    // as this is the only action they will currently perform.
                    var hashedAccountId = context.Request.Query.Get("HashedAccountId");
                    var requestRedirect = string.IsNullOrEmpty(hashedAccountId) ? "/service/index" : $"/accounts/{hashedAccountId}/teams/view";
                    
                    context.Authentication.Challenge(new AuthenticationProperties
                    { RedirectUri = requestRedirect, IsPersistent = true },
                        "Staff"
                    );

                    context.Response.StatusCode = 401;
                    return context.Response.WriteAsync(string.Empty);
                });
            });

            app.UseCodeFlowAuthentication(new OidcMiddlewareOptions
            {
                AuthenticationType = "Cookies",
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
                    PostAuthentiationAction(
                        identity,
                        constants,
                        accountDataCookieStorageService,
                        hashedAccountIdCookieStorageService);
                }
            });

            app.Map($"/login", conf =>
            {
                conf.Run(context =>
                {
                    context.Authentication.Challenge(new AuthenticationProperties
                    { RedirectUri = "/service/index", IsPersistent = true },
                        "Cookies"
                    );

                    context.Response.StatusCode = 401;
                    return context.Response.WriteAsync(string.Empty);
                });
            });

            ConfigurationFactory.Current = new IdentityServerConfigurationFactory(config);
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            UserLinksViewModel.ChangePasswordLink = $"{constants.ChangePasswordLink()}{urlHelper.Encode(config.EmployerAccountsBaseUrl + "/service/password/change")}";
            UserLinksViewModel.ChangeEmailLink = $"{constants.ChangeEmailLink()}{urlHelper.Encode(config.EmployerAccountsBaseUrl + "/service/email/change")}";
        }

        private WsFederationAuthenticationOptions GetADFSOptions()
        {
            return new WsFederationAuthenticationOptions
            {
                AuthenticationType = "Staff",
                Wtrealm = "https://localhost:44347",
                MetadataAddress = "https://adfs.preprod.skillsfunding.service.gov.uk/FederationMetadata/2007-06/FederationMetadata.xml",
                Notifications = Notifications()
            };
        }

        private WsFederationAuthenticationNotifications Notifications()
        {
            return new WsFederationAuthenticationNotifications
            {
                SecurityTokenValidated = OnSecurityTokenValidated,
                SecurityTokenReceived = nx => OnSecurityTokenReceived(),
                AuthenticationFailed = nx => OnAuthenticationFailed(nx),
                MessageReceived = nx => OnMessageReceived(),
                RedirectToIdentityProvider = nx => OnRedirectToIdentityProvider()
            };
        }

        private Task OnRedirectToIdentityProvider()
        {
            Logger.Debug("RedirectToIdentityProvider");
            return Task.FromResult(0);
        }

        private Task OnMessageReceived()
        {
            Logger.Debug("MessageReceived");
            return Task.FromResult(0);
        }

        private Task OnSecurityTokenReceived()
        {
            Logger.Debug("SecurityTokenReceived");
            return Task.FromResult(0);
        }

        private Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification)
        {
            Logger.Debug("SecurityTokenValidated");

            try
            {
                Logger.Debug("Authentication Properties", new Dictionary<string, object>
                {
                    {
                        "claims",
                        JsonConvert.SerializeObject(
                            notification.AuthenticationTicket.Identity.Claims.Select(x =>new {x.Value, x.ValueType, x.Type}))
                    },
                    {
                        "authentication-type",
                        notification.AuthenticationTicket.Identity.AuthenticationType
                    },
                    {"role-type", notification.AuthenticationTicket.Identity.RoleClaimType}
                });

                const string serviceClaimType = "http://service/service";

                if (notification.AuthenticationTicket.Identity.HasClaim(serviceClaimType, "ESF"))
                {
                    Logger.Debug("Adding Tier2 Role");
                    notification.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, "Tier2User"));

                    Logger.Debug("Adding ConsoleUser Role");
                    notification.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, "ConsoleUser"));
                }
                else if (notification.AuthenticationTicket.Identity.HasClaim(serviceClaimType, "ESS"))
                {
                    Logger.Debug("Adding ConsoleUser Role");
                    notification.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, "ConsoleUser"));
                }
                else
                {
                    throw new SecurityTokenValidationException();
                }

                var firstName = notification.AuthenticationTicket.Identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var lastName = notification.AuthenticationTicket.Identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                var userEmail = notification.AuthenticationTicket.Identity.Claims.Single(x => x.Type == ClaimTypes.Upn).Value;

                notification.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Email, userEmail));
                notification.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "IDAMS Authentication Callback Error");
            }

            Logger.Debug("End of callback");

            return Task.FromResult(0);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<WsFederationMessage, WsFederationAuthenticationOptions> nx)
        {
            var logReport = $"AuthenticationFailed, State: {nx.State}, Exception: {nx.Exception.GetBaseException().Message}, Protocol Message: Wct {nx.ProtocolMessage.Wct},\r\nWfresh {nx.ProtocolMessage.Wfresh},\r\nWhr {nx.ProtocolMessage.Whr},\r\nWp {nx.ProtocolMessage.Wp},\r\nWpseudo{nx.ProtocolMessage.Wpseudo},\r\nWpseudoptr {nx.ProtocolMessage.Wpseudoptr},\r\nWreq {nx.ProtocolMessage.Wreq},\r\nWfed {nx.ProtocolMessage.Wfed},\r\nWreqptr {nx.ProtocolMessage.Wreqptr},\r\nWres {nx.ProtocolMessage.Wres},\r\nWreply{nx.ProtocolMessage.Wreply},\r\nWencoding {nx.ProtocolMessage.Wencoding},\r\nWtrealm {nx.ProtocolMessage.Wtrealm},\r\nWresultptr {nx.ProtocolMessage.Wresultptr},\r\nWauth {nx.ProtocolMessage.Wauth},\r\nWattrptr{nx.ProtocolMessage.Wattrptr},\r\nWattr {nx.ProtocolMessage.Wattr},\r\nWa {nx.ProtocolMessage.Wa},\r\nIsSignOutMessage {nx.ProtocolMessage.IsSignOutMessage},\r\nIsSignInMessage {nx.ProtocolMessage.IsSignInMessage},\r\nWctx {nx.ProtocolMessage.Wctx},\r\n";
            Logger.Debug(logReport);
            return Task.FromResult(0);
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
                    var thumbprint = CloudConfigurationManager.GetSetting("TokenCertificateThumbprint");
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

        private static void PostAuthentiationAction(ClaimsIdentity identity,
            Constants constants,
            ICookieStorageService<EmployerAccountData> accountDataCookieStorageService,
            ICookieStorageService<HashedAccountIdModel> hashedAccountIdCookieStorageService)
        {
            Logger.Info("Retrieving claims from OIDC server.");

            var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;
          
            Logger.Info($"Retrieved claims from OIDC server for user with external ID '{userRef}'.");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identity.Claims.First(c => c.Type == constants.Id()).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, identity.Claims.First(c => c.Type == constants.DisplayName()).Value));
            identity.AddClaim(new Claim("sub", identity.Claims.First(c => c.Type == constants.Id()).Value));
            identity.AddClaim(new Claim("email", identity.Claims.First(c => c.Type == constants.Email()).Value));
            identity.AddClaim(new Claim("firstname", identity.Claims.First(c => c.Type == constants.GivenName()).Value));
            identity.AddClaim(new Claim("lastname", identity.Claims.First(c => c.Type == constants.FamilyName()).Value));

            Task.Run(() => accountDataCookieStorageService.Delete(AccountDataCookieName)).Wait();
            Task.Run(() => hashedAccountIdCookieStorageService.Delete(typeof(HashedAccountIdModel).FullName)).Wait();
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
        public string RegisterLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.RegisterLink, _configuration.ClientId);
        public string RequiresVerification() => _baseUrl + "requires_verification";
        public string TokenEndpoint() => $"{_configuration.BaseAddress}{_configuration.TokenEndpoint}";
        public string UserInfoEndpoint() => $"{_configuration.BaseAddress}{_configuration.UserInfoEndpoint}";
    }
}