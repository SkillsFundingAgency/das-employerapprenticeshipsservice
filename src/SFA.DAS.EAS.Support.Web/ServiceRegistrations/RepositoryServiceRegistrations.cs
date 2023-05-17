using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Infrastructure.Services;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

[ExcludeFromCodeCoverage]
public static class RepositoryServiceRegistrations
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ILevySubmissionsRepository, LevySubmissionsRepository>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IChallengeRepository, ChallengeRepository>();

        return services;
    }
}
