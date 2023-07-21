using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class SupportUserExtensions
    {
        private const string Staff = "SFA.Staff.Auth";
        private const string HashedAccountId = "HashedAccountId";
        private const string ServiceClaimTier2Role = "ESF";
        private const string ConsoleUser = "ConsoleUser";
        private const string ServiceClaimTier1Role = "ESS";
        private const string serviceClaimType = "http://service/service";

        public static AuthenticationBuilder AddAndConfigureSupportConsoleAuthentication(this AuthenticationBuilder services, SupportConsoleAuthenticationOptions authOptions)
        {
            services
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = authOptions.AdfsOptions.MetadataAddress;
                    options.Wtrealm = authOptions.AdfsOptions.Wtrealm;
                    // set /service to fix issues
                    options.Wreply = GetServiceUrl(authOptions.AdfsOptions.Wreply);
                    options.UseTokenLifetime = false;
                    options.CallbackPath = PathString.Empty;
                    options.CorrelationCookie = new CookieBuilder
                    {
                        Name = Staff,
                        SameSite = SameSiteMode.None,
                        HttpOnly = false,
                        SecurePolicy = CookieSecurePolicy.Always
                    };
                    options.Events = new WsFederationEvents
                    {
                        OnSecurityTokenValidated = OnSecurityTokenValidated,
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

        public static IApplicationBuilder UseSupportConsoleAuthentication(this IApplicationBuilder app)
        {
            // https://skillsfundingagency.atlassian.net/wiki/spaces/ERF/pages/104010807/Staff+IDAMS
            app.Map("/login/staff", SetAuthenticationContextForStaffUser());

            return app;
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


                        await context.ChallengeAsync(WsFederationDefaults.AuthenticationScheme, new AuthenticationProperties
                        {
                            RedirectUri = requestRedirect,
                            IsPersistent = true
                        });
                    }
                    else
                    {
                        context.Response.Redirect(requestRedirect); // Redirect to the home page or any other desired location
                    }
                });
            };
        }

        private static Task OnSecurityTokenValidated(SecurityTokenValidatedContext context)
        {
            var redirectUri = context.Properties.RedirectUri;

            // Retrieve the HashedAccountId query parameter
            var hashedAccountId = redirectUri.Split('/')[2];

            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
            var db = context.HttpContext.RequestServices.GetService(typeof(EmployerAccountsDbContext)) as EmployerAccountsDbContext;
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
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, SupportUserClaimConstants.Tier2UserClaim));

                logger?.LogDebug("Adding ConsoleUser Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
            }
            else if (claimsIdentity.HasClaim(serviceClaimType, ServiceClaimTier1Role))
            {
                logger?.LogDebug("Adding Tier1 Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, SupportUserClaimConstants.Tier1UserClaim));

                logger?.LogDebug("Adding ConsoleUser Role");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, ConsoleUser));
            }
            else
            {
                throw new SecurityTokenValidationException("Service Claim Type not available to identify the Role");
            }

            ImpersonateExistingAccountOwner(hashedAccountId, db, claimsIdentity);

            var firstName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claimsIdentity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            var userEmail = claimsIdentity.Claims.Single(x => x.Type == ClaimTypes.Upn).Value;

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));

            if (!string.IsNullOrEmpty(userEmail))
            {
                claimsIdentity.AddClaim(new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, userEmail));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userEmail));
            }
            else
            {
                throw new SecurityTokenValidationException("Upn not available in the claims to identify UserEmail");
            }

            logger?.LogDebug("End of callback");

            return Task.CompletedTask;
        }

        private static void ImpersonateExistingAccountOwner(string hashedAccountId, EmployerAccountsDbContext db, ClaimsIdentity claimsIdentity)
        {
            var accountOwner = db.Accounts.Single(acc => acc.HashedId == hashedAccountId).Memberships.FirstOrDefault(m => m.Role == Role.Owner).User;
            claimsIdentity.AddClaim(new Claim(ControllerConstants.UserRefClaimKeyName, accountOwner.Ref.ToString()));
        }

        private static string GetServiceUrl(string wreply)
        {
            return $"{(wreply.EndsWith('/') ? wreply : wreply + "/")}staff";
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

    public static class SupportUserClaimConstants
    {
        public const string WsFederationAuthScheme = "wsfed";
        public const string Tier2UserClaim = "Tier2User";
        public const string Tier1UserClaim = "Tier1User";
    }
}
