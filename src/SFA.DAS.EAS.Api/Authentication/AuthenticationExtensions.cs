using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration config, bool isDevelopment = false)
    {
        if (isDevelopment)
        {
            services.AddAuthentication("BasicAuthentication")
                   .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }
        else
        {
            var azureAdConfiguration = config
                   .GetSection(ConfigurationKeys.AzureActiveDirectoryApiConfiguration)
                   .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string> {
                { PolicyNames.Default, RoleNames.Default },
                { ApiRoles.ReadAllEmployerAccountBalances, ApiRoles.ReadAllEmployerAccountBalances },
                { ApiRoles.ReadUserAccounts,ApiRoles.ReadUserAccounts },
                { ApiRoles.ReadAllAccountUsers,ApiRoles.ReadAllAccountUsers },
                { ApiRoles.ReadAllEmployerAgreements, ApiRoles.ReadAllEmployerAgreements }
            };
            
            services.AddAuthentication(azureAdConfiguration, policies);
        }

        return services;
    }
}
