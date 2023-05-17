using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.Web.Configuration;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class AuthenticationServiceRegistrations
{

    public static IServiceCollection AddAndConfigureSupportAuthentication(this IServiceCollection services, EasSupportConfiguration configuration)
    {
        services.AddAuthorization(o =>
        {
            o.AddPolicy("default", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("das-support-portal");
            });
        });

        services.AddAuthentication(auth =>
        {
            auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(auth =>
        {
            auth.Authority = $"https://login.microsoftonline.com/{configuration.SiteValidator.Tenant}";
            auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidAudiences = configuration.SiteValidator.Audience.Split(','),
            };
        });

        return services;
    }
}
