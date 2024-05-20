using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
                auth.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = configuration.SiteValidator.Audience.Split(','),
                };
            });

        return services;
    }
}