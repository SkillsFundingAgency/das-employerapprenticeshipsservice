//using Microsoft.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Cookies;
//using NLog;
//using Owin;
//using SFA.DAS.Authentication;
//using SFA.DAS.EmployerAccounts.Configuration;
//using SFA.DAS.EmployerAccounts.Interfaces;
//using SFA.DAS.EmployerAccounts.Models.Account;
//using SFA.DAS.EmployerAccounts.Web;
//using SFA.DAS.EmployerAccounts.Web.Authentication;
//using SFA.DAS.EmployerAccounts.Web.Extensions;
//using SFA.DAS.EmployerAccounts.Web.Models;
//using SFA.DAS.EmployerUsers.WebClientComponents;
//using SFA.DAS.OidcMiddleware;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IdentityModel.Tokens;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography.X509Certificates;
//using System.Threading.Tasks;

//[assembly: OwinStartup(typeof(Startup))]

//namespace SFA.DAS.EmployerAccounts.Web
//{
//    public class Startup_old
//    {
//        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
//        private const string AccountDataCookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";        
//        private const string CookieAuthenticationTypeTempState = "TempState";

//        public void Configuration(IAppBuilder app)
//        {           
//            var config = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<EmployerAccountsConfiguration>();
//            var accountDataCookieStorageService = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ICookieStorageService<EmployerAccountData>>();
//            var hashedAccountIdCookieStorageService = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ICookieStorageService<HashedAccountIdModel>>();
//            var constants = new Constants(config.Identity);

//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
//                ExpireTimeSpan = new TimeSpan(0, 10, 0),
//                SlidingExpiration = true
//            });

//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationType = CookieAuthenticationTypeTempState,
//                AuthenticationMode = AuthenticationMode.Passive
//            });

//app.UseSupportConsoleAuthentication(new SupportConsoleAuthenticationOptions
//{
//    AdfsOptions = new ADFSOptions
//    {

//        MetadataAddress = config.AdfsMetadata,
//        Wreply = config.EmployerAccountsBaseUrl,
//        Wtrealm = config.EmployerAccountsBaseUrl,
//        BaseUrl = config.Identity.BaseAddress,

//    },
//    Logger = Logger
//});

//            app.UseCodeFlowAuthentication(GetOidcMiddlewareOptions(config, accountDataCookieStorageService, hashedAccountIdCookieStorageService, constants));

//            ConfigurationFactory.Current = new IdentityServerConfigurationFactory(config);
//            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
//        }    

//        private static OidcMiddlewareOptions GetOidcMiddlewareOptions(EmployerAccountsConfiguration config,
//            ICookieStorageService<EmployerAccountData> accountDataCookieStorageService,
//            ICookieStorageService<HashedAccountIdModel> hashedAccountIdCookieStorageService,
//            Constants constants)
//        {
//            return new OidcMiddlewareOptions
//            {
//                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
//                BaseUrl = config.Identity.BaseAddress,
//                ClientId = config.Identity.ClientId,
//                ClientSecret = config.Identity.ClientSecret,
//                Scopes = config.Identity.Scopes,
//                AuthorizeEndpoint = constants.AuthorizeEndpoint(),
//                TokenEndpoint = constants.TokenEndpoint(),
//                UserInfoEndpoint = constants.UserInfoEndpoint(),
//                TokenSigningCertificateLoader = GetSigningCertificate(config.Identity.UseCertificate),
//                TokenValidationMethod = config.Identity.UseCertificate ? TokenValidationMethod.SigningKey : TokenValidationMethod.BinarySecret,
//                AuthenticatedCallback = identity =>
//                {
//                    PostAuthentiationAction(
//                        identity,
//                        constants,
//                        accountDataCookieStorageService,
//                        hashedAccountIdCookieStorageService);
//                }
//            };
//        }

//        private static Func<X509Certificate2> GetSigningCertificate(bool useCertificate)
//        {
//            if (!useCertificate)
//            {
//                return null;
//            }

//            return () =>
//            {
//                var store = new X509Store(StoreLocation.CurrentUser);

//                store.Open(OpenFlags.ReadOnly);

//                try
//                {
//                    var thumbprint = ConfigurationManager.AppSettings["TokenCertificateThumbprint"];
//                    var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

//                    if (certificates.Count < 1)
//                    {
//                        throw new Exception($"Could not find certificate with thumbprint '{thumbprint}' in CurrentUser store.");
//                    }

//                    return certificates[0];
//                }
//                finally
//                {
//                    store.Close();
//                }
//            };
//        }

//        private static void PostAuthentiationAction(ClaimsIdentity identity,
//            Constants constants,
//            ICookieStorageService<EmployerAccountData> accountDataCookieStorageService,
//            ICookieStorageService<HashedAccountIdModel> hashedAccountIdCookieStorageService)
//        {
//            Logger.Info("Retrieving claims from OIDC server.");

//            var userRef = identity.Claims.FirstOrDefault(claim => claim.Type == constants.Id())?.Value;

//            Logger.Info($"Retrieved claims from OIDC server for user with external ID '{userRef}'.");

//            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identity.Claims.First(c => c.Type == constants.Id()).Value));
//            identity.AddClaim(new Claim(ClaimTypes.Name, identity.Claims.First(c => c.Type == constants.DisplayName()).Value));
//            identity.AddClaim(new Claim("sub", identity.Claims.First(c => c.Type == constants.Id()).Value));
//            identity.AddClaim(new Claim("email", identity.Claims.First(c => c.Type == constants.Email()).Value));
//            identity.AddClaim(new Claim("firstname", identity.Claims.First(c => c.Type == constants.GivenName()).Value));
//            identity.AddClaim(new Claim("lastname", identity.Claims.First(c => c.Type == constants.FamilyName()).Value));

//            Task.Run(() => accountDataCookieStorageService.Delete(AccountDataCookieName)).Wait();
//            Task.Run(() => hashedAccountIdCookieStorageService.Delete(typeof(HashedAccountIdModel).FullName)).Wait();
//        }
//    }

//    public class Constants
//    {
//        private readonly string _baseUrl;
//        private readonly IdentityServerConfiguration _configuration;

//        public Constants(IdentityServerConfiguration configuration)
//        {
//            _baseUrl = configuration.ClaimIdentifierConfiguration.ClaimsBaseUrl;
//            _configuration = configuration;
//        }

//        public string AuthorizeEndpoint() => $"{_configuration.BaseAddress}{_configuration.AuthorizeEndPoint}";
//        public string ChangeEmailLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangeEmailLink, _configuration.ClientId);
//        public string ChangePasswordLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.ChangePasswordLink, _configuration.ClientId);
//        public string DisplayName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.DisplayName;
//        public string Email() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Email;
//        public string FamilyName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.FaimlyName;
//        public string GivenName() => _baseUrl + _configuration.ClaimIdentifierConfiguration.GivenName;
//        public string Id() => _baseUrl + _configuration.ClaimIdentifierConfiguration.Id;
//        public string LogoutEndpoint() => $"{_configuration.BaseAddress}{_configuration.LogoutEndpoint}";
//        public string RegisterLink() => _configuration.BaseAddress.Replace("/identity", "") + string.Format(_configuration.RegisterLink, _configuration.ClientId);
//        public string RequiresVerification() => _baseUrl + "requires_verification";
//        public string TokenEndpoint() => $"{_configuration.BaseAddress}{_configuration.TokenEndpoint}";
//        public string UserInfoEndpoint() => $"{_configuration.BaseAddress}{_configuration.UserInfoEndpoint}";
//    }
//}