using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;

public static class OrchestratorsServiceRegistrations
{
    public static IServiceCollection AddOrchestrators(this IServiceCollection services)
    {
        services.AddTransient<AccountsOrchestrator>();
        services.AddTransient<AgreementOrchestrator>();
        services.AddTransient<UsersOrchestrator>();

        return services;
    }
}