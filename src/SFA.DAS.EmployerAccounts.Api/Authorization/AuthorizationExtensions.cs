using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerAccounts.Api.Authorization;

public static class AuthorizationExtensions
{
    private static readonly string[] PolicyRoles =
    {
        ApiRoles.ReadAllEmployerAccountBalances,
        ApiRoles.ReadUserAccounts,
        ApiRoles.ReadAllAccountUsers,
        ApiRoles.ReadAllEmployerAgreements
    };

    private const string DefaultPolicyName = "default";

    public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
    {
        services.AddAuthorization(options =>
        {
            AddDefaultPolicy(isDevelopment, options);

            AddRolePolicies(isDevelopment, options);

            options.DefaultPolicy = options.GetPolicy(DefaultPolicyName);

        });

        if (isDevelopment)
        {
            services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();
        }

        return services;
    }

    private static void AddRolePolicies(bool isDevelopment, AuthorizationOptions options)
    {
        foreach (var roleName in PolicyRoles)
        {
            options.AddPolicy(roleName, policy =>
            {
                if (isDevelopment)
                {
                    policy.AllowAnonymousUser();
                }
                else
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(roleName);
                }
            });
        }
    }

    private static void AddDefaultPolicy(bool isDevelopment, AuthorizationOptions x)
    {
        x.AddPolicy(DefaultPolicyName, policy =>
        {
            if (isDevelopment)
            {
                policy.AllowAnonymousUser();
            }
            else
            {
                policy.RequireAuthenticatedUser();
            }
        });
    }
}