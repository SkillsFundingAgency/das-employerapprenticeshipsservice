using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Authentication;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.Cookies;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Handlers;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class EmployerAuthenticationServiceRegistrations
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<ICustomClaims, EmployerAccountPostAuthenticationClaimsHandler>();
        services.AddTransient<IEmployerAccountAuthorisationHandler, EmployerAccountAuthorisationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountAllRolesAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerUsersIsOutsideAccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountOwnerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>();//TODO remove after gov login enabled
        services.AddTransient<IUserAccountService, UserAccountService>();


        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PolicyNames.HasUserAccount
                , policy =>
                {
                    policy.RequireClaim(EmployerClaims.IdamsUserIdClaimTypeIdentifier);
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AccountActiveRequirement());
                });

            options.AddPolicy(
                PolicyNames.HasEmployerOwnerAccount
                , policy =>
                {
                    policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                    policy.Requirements.Add(new EmployerAccountOwnerRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                    policy.RequireAuthenticatedUser();
                });
            options.AddPolicy(
                PolicyNames.HasEmployerViewerTransactorOwnerAccount
                , policy =>
                {
                    policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                    policy.Requirements.Add(new EmployerAccountAllRolesRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                    policy.RequireAuthenticatedUser();
                });
            options.AddPolicy(
                PolicyNames.HasEmployerViewerTransactorOwnerAccountOrSupport
                , policy =>
                {
                    policy.RequireAssertion(_ =>
                    {
                        if (_.User.IsSupportUser())
                        {
                            return true;
                        }

                        policy.Requirements.Add(new EmployerAccountAllRolesRequirement());
                        policy.Requirements.Add(new AccountActiveRequirement());

                        return _.User.HasClaim(c => c.Type == EmployerClaims.AccountsClaimsTypeIdentifier);
                    });
                    policy.RequireAuthenticatedUser();
                });
        });

        return services;
    }

    private static bool IsSupportUser(this ClaimsPrincipal user)
    {
        return user.HasClaim(ClaimTypes.Role, SupportUserClaimConstants.Tier1UserClaim) || user.HasClaim(ClaimTypes.Role, SupportUserClaimConstants.Tier2UserClaim);
    }

    public static IServiceCollection AddAndConfigureEmployerAuthentication(
            this IServiceCollection services,
            IdentityServerConfiguration configuration)
    {
        services
            .AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

            }).AddOpenIdConnect(options =>
            {
                options.ClientId = configuration.ClientId;
                options.ClientSecret = configuration.ClientSecret;
                options.Authority = configuration.BaseAddress;
                options.MetadataAddress = $"{configuration.BaseAddress}/.well-known/openid-configuration";
                options.ResponseType = "code";
                options.UsePkce = false;

                var scopes = configuration.Scopes.Split(' ');
                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope);
                }
                options.ClaimActions.MapUniqueJsonKey("sub", "id");
                options.Events.OnRemoteFailure = c =>
                {
                    if (c.Failure.Message.Contains("Correlation failed"))
                    {
                        c.Response.Redirect("/");
                        c.HandleResponse();
                    }

                    return Task.CompletedTask;
                };
            }).AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = CookieNames.Authentication;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
            });

        services
            .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<ICustomClaims>((options, customClaims) =>
            {
                options.Events.OnTokenValidated = async (ctx) =>
                {
                    var claims = await customClaims.GetClaims(ctx);
                    ctx.Principal.Identities.First().AddClaims(claims);
                };
            });

        return services;
    }
}
