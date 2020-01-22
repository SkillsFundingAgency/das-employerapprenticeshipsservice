using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using Newtonsoft.Json;
using NLog;
using Owin;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using NLog;
using Microsoft.Owin.Security;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class SupportUserExtensions
    {
        private const string Cookies = "Cookies";
        private const string TempState = "TempState";
        private const string Staff = "Staff";
        private const string Employer = "Employer";
        private const string HashedAccountId = "HashedAccountId";
        private const string ESF = "ESF";
        private const string Tier2User = "Tier2User";
        private const string ConsoleUser = "ConsoleUser";
        private const string ESS = "ESS";
        private const string serviceClaimType = "http://service/service";
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static IAppBuilder UseSupportConsoleAuthentication(this IAppBuilder app, SupportConsoleAuthenticationOptions options)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Staff,
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Employer,
                ExpireTimeSpan = new TimeSpan(0, 10, 0),
                SlidingExpiration = true
            });

            var WsFederationAuthenticationOptions = new WsFederationAuthenticationOptions
            {
                AuthenticationType = Staff,
                Wtrealm = options.AdfsOptions.Wtrealm,
                MetadataAddress = options.AdfsOptions.MetadataAddress,
                Notifications = Notifications(),
                Wreply = options.AdfsOptions.Wreply
            };
            
            app.UseWsFederationAuthentication(WsFederationAuthenticationOptions);

            app.Map($"/login/staff", SetAuthenticationContextForStaffUser());

            return app;

        }

        private static WsFederationAuthenticationNotifications Notifications()
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

        private static Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification)
        {
            Logger.Debug("SecurityTokenValidated");

            try
            {
                var claimsIdentity = notification.AuthenticationTicket.Identity;

                Logger.Debug("Authentication Properties", new Dictionary<string, object>
                {
                    {"claims", JsonConvert.SerializeObject(claimsIdentity.Claims.Select(x =>new {x.Value, x.ValueType, x.Type}))},
                    {"authentication-type", claimsIdentity.AuthenticationType},
                    {"role-type", claimsIdentity.RoleClaimType}
                });

                if (notification.AuthenticationTicket.Identity.HasClaim(serviceClaimType, ESF))
                {
                    Logger.Debug("Adding Tier2 Role");
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Tier2User));

                    Logger.Debug("Adding ConsoleUser Role");
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
                }
                else if (notification.AuthenticationTicket.Identity.HasClaim(serviceClaimType, ESS))
                {
                    Logger.Debug("Adding ConsoleUser Role");
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
                }
                else
                {
                    throw new SecurityTokenValidationException();
                }

                var firstName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var lastName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                var userEmail = claimsIdentity.Claims.Single(x => x.Type == ClaimTypes.Upn).Value;

                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, userEmail));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userEmail));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "IDAMS Authentication Callback Error");
            }

            Logger.Debug("End of callback");

            return Task.FromResult(0);
        }

        private static Task OnSecurityTokenReceived()
        {
            Logger.Debug("SecurityTokenReceived");
            return Task.FromResult(0);
        }

        private static Task OnAuthenticationFailed(AuthenticationFailedNotification<WsFederationMessage, WsFederationAuthenticationOptions> nx)
        {
            var logReport = $"AuthenticationFailed, State: {nx.State}, Exception: {nx.Exception.GetBaseException().Message}, Protocol Message: Wct {nx.ProtocolMessage.Wct},\r\nWfresh {nx.ProtocolMessage.Wfresh},\r\nWhr {nx.ProtocolMessage.Whr},\r\nWp {nx.ProtocolMessage.Wp},\r\nWpseudo{nx.ProtocolMessage.Wpseudo},\r\nWpseudoptr {nx.ProtocolMessage.Wpseudoptr},\r\nWreq {nx.ProtocolMessage.Wreq},\r\nWfed {nx.ProtocolMessage.Wfed},\r\nWreqptr {nx.ProtocolMessage.Wreqptr},\r\nWres {nx.ProtocolMessage.Wres},\r\nWreply{nx.ProtocolMessage.Wreply},\r\nWencoding {nx.ProtocolMessage.Wencoding},\r\nWtrealm {nx.ProtocolMessage.Wtrealm},\r\nWresultptr {nx.ProtocolMessage.Wresultptr},\r\nWauth {nx.ProtocolMessage.Wauth},\r\nWattrptr{nx.ProtocolMessage.Wattrptr},\r\nWattr {nx.ProtocolMessage.Wattr},\r\nWa {nx.ProtocolMessage.Wa},\r\nIsSignOutMessage {nx.ProtocolMessage.IsSignOutMessage},\r\nIsSignInMessage {nx.ProtocolMessage.IsSignInMessage},\r\nWctx {nx.ProtocolMessage.Wctx},\r\n";
            Logger.Debug(logReport);
            return Task.FromResult(0);
        }

        private static Task OnMessageReceived()
        {
            Logger.Debug("MessageReceived");
            return Task.FromResult(0);
        }

        private static  Task OnRedirectToIdentityProvider()
        {
            Logger.Debug("RedirectToIdentityProvider");
            return Task.FromResult(0);
        }

        private static Action<IAppBuilder> SetAuthenticationContextForStaffUser()
        {
            return conf =>
            {
                conf.Run(context =>
                {
                    // for first iteration of this work, allow deep linking from the support console to the teams view
                    // as this is the only action they will currently perform.
                    var hashedAccountId = context.Request.Query.Get(HashedAccountId);
                    var requestRedirect = string.IsNullOrEmpty(hashedAccountId) ? "/service/index" : $"/accounts/{hashedAccountId}/teams/view";

                    context.Authentication.Challenge(new AuthenticationProperties
                    {
                        RedirectUri = requestRedirect,
                        IsPersistent = true
                    },
                    Staff);

                    context.Response.StatusCode = 401;
                    return context.Response.WriteAsync(string.Empty);
                });
            };
        }

    }


    public class SupportConsoleAuthenticationOptions
    {
        public ADFSOptions AdfsOptions { get; set; }
    }

    public class ADFSOptions
    {
        public string Wtrealm { get; set; }
        public string MetadataAddress { get; set; }
        public string Wreply { get; set; }

        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string Scopes { get; set; }

        public bool UseCertificate { get; set; }

    }   
}