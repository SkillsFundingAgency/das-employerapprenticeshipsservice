using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerAccounts.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
        {
            services.AddAuthorization(x =>
            {
                {
                    x.AddPolicy("default", policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                            policy.RequireAuthenticatedUser();

                    });

                    x.AddPolicy(ApiRoles.ReadAllEmployerAccountBalances, policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(ApiRoles.ReadAllEmployerAccountBalances);
                        }
                    });

                    x.AddPolicy(ApiRoles.ReadUserAccounts, policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(ApiRoles.ReadUserAccounts);
                        }
                    });

                    x.AddPolicy(ApiRoles.ReadAllAccountUsers, policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(ApiRoles.ReadAllAccountUsers);
                        }
                    });
                    
                    x.AddPolicy(ApiRoles.ReadAllEmployerAgreements, policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(ApiRoles.ReadAllEmployerAgreements);
                        }
                    });

                    x.DefaultPolicy = x.GetPolicy("default");
                }
            });
            if (isDevelopment)
                services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();

            return services;
        }
    }
}
