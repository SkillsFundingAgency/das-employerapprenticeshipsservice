using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.Infrastructure.Config;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Configuration;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class AuthenticationServiceRegistrations
{
    public static IServiceCollection AddActiveDirectoryAuthentication(this IServiceCollection services,
        EasSupportConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.Default, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleNames.SupportPortal);
            });
        });

        services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(auth =>
            {
                auth.Authority = $"https://login.microsoftonline.com/{configuration.SiteValidator.Tenant}";
                auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudiences = configuration.SiteValidator.Audience.Split(','),
                };
            });
        services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();

        return services;
    }
}