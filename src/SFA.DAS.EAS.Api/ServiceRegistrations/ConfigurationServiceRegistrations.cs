using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddApiConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("EmployerAccountsApi").Get<EmployerAccountsApiConfiguration>());
        services.AddSingleton(configuration.GetSection("EmployerFinanceApi").Get<EmployerFinanceApiConfiguration>());

        return services;
    }
}
