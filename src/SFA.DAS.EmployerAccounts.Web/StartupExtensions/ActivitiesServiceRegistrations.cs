using Nest;
using SFA.DAS.Activities.Client;
using SFA.DAS.Elastic;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ActivitiesServiceRegistrations
{
    public static IServiceCollection AddActivitiesClient(this IServiceCollection services)
    {
        services.AddTransient<IActivitiesClient, ActivitiesClient>();

        services.AddSingleton<IElasticClientFactory>(provider =>
        {
            var config = provider.GetService<ElasticConfiguration>();
            return config.CreateClientFactory();
        });

        services.AddSingleton<IElasticClient>(provider =>
        {
            var factory = provider.GetService<IElasticClientFactory>();
            return factory.CreateClient();
        });

        return services;
    }
}