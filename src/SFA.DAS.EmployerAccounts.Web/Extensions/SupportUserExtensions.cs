using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class SupportUserExtensions
    {
        private const string Staff = "Staff";
        private const string Employer = "Employer";
        private const string HashedAccountId = "HashedAccountId";
        private const string ServiceClaimTier2Role = "ESF";
        private const string Tier2User = "Tier2User";
        private const string Tier1User = "Tier1User";
        private const string ConsoleUser = "ConsoleUser";
        private const string ServiceClaimTier1Role = "ESS";
        private const string serviceClaimType = "http://service/service";
        private const string WsFed = "wsfed";

        public static AuthenticationBuilder AddAndConfigureSupportConsoleAuthentication(this AuthenticationBuilder services, SupportConsoleAuthenticationOptions authOptions)
        {
            services
                .AddWsFederation(WsFed, options =>
                {
                    options.SignInScheme = Staff;
                    options.MetadataAddress = authOptions.AdfsOptions.MetadataAddress;
                    options.Wtrealm = authOptions.AdfsOptions.Wtrealm;
                    options.Wreply = authOptions.AdfsOptions.Wreply;
                    options.UseTokenLifetime = false;
                    options.CallbackPath = PathString.Empty;
                    options.CorrelationCookie = new CookieBuilder
                    {
                        Name = "SFA.Staff.Auth",
                        SameSite = SameSiteMode.None,
                        HttpOnly = false,
                        SecurePolicy = CookieSecurePolicy.Always
                    };
                    options.Events = new WsFederationEvents
                    {
                        OnRemoteFailure = OnRemoteFailure,
                        OnAccessDenied = OnAccessDenied,
                        OnMessageReceived = OnMessageReceived,
                        OnTicketReceived = OnTicketReceived,
                        OnSecurityTokenValidated = OnSecurityTokenValidated,
                        OnSecurityTokenReceived = OnSecurityTokenReceived,
                        OnAuthenticationFailed = OnAuthenticationFailed,
                        OnRedirectToIdentityProvider = OnRedirectToIdentityProvider
                    };
                })
                .AddCookie(Staff, options =>
                {
                    options.AccessDeniedPath = new PathString("/Error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = Staff;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = false;
                    options.Cookie.SameSite = SameSiteMode.None;
                });

            return services;
        }

        private static Task OnTicketReceived(TicketReceivedContext context)
        {
            return Task.CompletedTask;
        }

        private static Task OnMessageReceived(MessageReceivedContext context)
        {
            return Task.CompletedTask;
        }

        private static Task OnRemoteFailure(RemoteFailureContext context)
        {
            if (context.Failure.Message.Contains("Correlation failed", StringComparison.InvariantCultureIgnoreCase)
                || context.Failure.Message.Contains("No message", StringComparison.InvariantCultureIgnoreCase))
            {
                //context.Response.Redirect("/");
                context.SkipHandler();
            }

            return Task.CompletedTask;
        }

        private static Task OnAccessDenied(AccessDeniedContext context)
        {
            return Task.CompletedTask;
        }

        public static IApplicationBuilder UseSupportConsoleAuthentication(this IApplicationBuilder app)
        {
            // https://skillsfundingagency.atlassian.net/wiki/spaces/ERF/pages/104010807/Staff+IDAMS
            //app.UseW();
            //app.UseWsFederationAuthentication(wsFederationOptions);

            app.Map("/login/staff", SetAuthenticationContextForStaffUser());

            //app.Map("/login", SetAuthenticationContext());

            return app;
        }

        private static Task OnSecurityTokenValidated(SecurityTokenValidatedContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
            logger?.LogDebug("SecurityTokenValidated");

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

            logger?.LogDebug("Authentication Properties", new Dictionary<string, object>
                {
                    {"claims", JsonConvert.SerializeObject(claimsIdentity.Claims.Select(x => new {x.Value, x.ValueType, x.Type}))},
                    {"authentication-type", claimsIdentity.AuthenticationType},
                    {"role-type", claimsIdentity.RoleClaimType}
                });

            if (claimsIdentity.HasClaim(serviceClaimType, ServiceClaimTier2Role))
            {
                logger?.LogDebug("Adding Tier2 Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Tier2User));

                logger?.LogDebug("Adding ConsoleUser Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
            }
            else if (claimsIdentity.HasClaim(serviceClaimType, ServiceClaimTier1Role))
            {
                logger?.LogDebug("Adding Tier1 Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Tier1User));

                logger?.LogDebug("Adding ConsoleUser Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
            }
            else
            {
                throw new SecurityTokenValidationException("Service Claim Type not available to identify the Role");
            }

            var firstName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            var userEmail = claimsIdentity.Claims.Single(x => x.Type == ClaimTypes.Upn).Value;

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));

            if (!string.IsNullOrEmpty(userEmail))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, userEmail));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userEmail));
            }
            else
            {
                throw new SecurityTokenValidationException("Upn not available in the claims to identify UserEmail");
            }

            logger?.LogDebug("End of callback");

            return Task.CompletedTask;
        }

        private static Task OnSecurityTokenReceived(SecurityTokenReceivedContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
            logger?.LogDebug("SecurityTokenReceived");
            return Task.CompletedTask;
        }

        private static Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
            var logReport = $"AuthenticationFailed, State: {context.ProtocolMessage.Wresult}, Exception: {context.Exception.GetBaseException().Message}, Protocol Message: Wct {context.ProtocolMessage.Wct},\r\nWfresh {context.ProtocolMessage.Wfresh},\r\nWhr {context.ProtocolMessage.Whr},\r\nWp {context.ProtocolMessage.Wp},\r\nWpseudo{context.ProtocolMessage.Wpseudo},\r\nWpseudoptr {context.ProtocolMessage.Wpseudoptr},\r\nWreq {context.ProtocolMessage.Wreq},\r\nWfed {context.ProtocolMessage.Wfed},\r\nWreqptr {context.ProtocolMessage.Wreqptr},\r\nWres {context.ProtocolMessage.Wres},\r\nWreply{context.ProtocolMessage.Wreply},\r\nWencoding {context.ProtocolMessage.Wencoding},\r\nWtrealm {context.ProtocolMessage.Wtrealm},\r\nWresultptr {context.ProtocolMessage.Wresultptr},\r\nWauth {context.ProtocolMessage.Wauth},\r\nWattrptr{context.ProtocolMessage.Wattrptr},\r\nWattr {context.ProtocolMessage.Wattr},\r\nWa {context.ProtocolMessage.Wa},\r\nIsSignOutMessage {context.ProtocolMessage.IsSignOutMessage},\r\nIsSignInMessage {context.ProtocolMessage.IsSignInMessage},\r\nWctx {context.ProtocolMessage.Wctx},\r\n";
            logger?.LogDebug(logReport);
            return Task.CompletedTask;
        }

        private static Task OnRedirectToIdentityProvider(RedirectContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
            logger?.LogDebug("RedirectToIdentityProvider");
           return Task.CompletedTask;
        }

        private static Action<IApplicationBuilder> SetAuthenticationContextForStaffUser()
        {
            return app =>
            {
                app.Run(async context =>
                {
                    var hashedAccountId = context.Request.Query[HashedAccountId];
                    var requestRedirect = string.IsNullOrEmpty(hashedAccountId) ? "/service/index" : $"/accounts/{hashedAccountId}/teams/view";

                    if (!context.User.Identity.IsAuthenticated)
                    {
                        var logger = context.RequestServices.GetService(typeof(ILogger)) as ILogger;
                        // for first iteration of this work, allow deep linking from the support console to the teams view
                        // as this is the only action they will currently perform.


                        await context.ChallengeAsync(WsFed, new AuthenticationProperties
                        {
                            RedirectUri = requestRedirect,
                        });
                    }
                    else
                    {
                        context.Response.Redirect(requestRedirect); // Redirect to the home page or any other desired location
                    }
                });
            };
        }

        private static Action<IApplicationBuilder> SetAuthenticationContext()
        {
            return conf =>
            {
                conf.Run(async context =>
                {
                    var logger = context.RequestServices.GetService(typeof(ILogger)) as ILogger;
                    await context.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties
                    {
                        RedirectUri = "/service/index",
                        IsPersistent = true
                    });

                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(string.Empty);
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
    }
}
