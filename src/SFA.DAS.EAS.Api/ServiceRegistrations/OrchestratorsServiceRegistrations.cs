using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class OrchestratorsServiceRegistrations
{
    public static IServiceCollection AddOrchestrators(this IServiceCollection services)
    {
        services.AddScoped<StatisticsOrchestrator>();
        services.AddScoped<AccountsOrchestrator>();
        services.AddScoped<AccountTransactionsOrchestrator>();

        return services;
    }
}
