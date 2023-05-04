using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.AuthPolicies;

namespace SFA.DAS.EAS.Account.Api.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("LoopBack", policy =>
            {
                policy.Requirements.Add(new LoopBackRequirement());
            });
        });

        if (isDevelopment)
        {
            services.AddSingleton<IAuthorizationHandler, LoopBackHandler>();
        }
        
        return services;
    }
}
