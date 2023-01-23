using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using NLog.Fluent;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.IndexMappers;
using SFA.DAS.Elastic;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ActivitiesServiceRegistrations
{
    public static IServiceCollection AddActivitiesClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ActivitiesClientConfiguration>(configuration.GetSection(nameof(ActivitiesClientConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ActivitiesClientConfiguration>>().Value);

        services.AddSingleton<ElasticConfiguration>(cfg =>
        {
            var config = cfg.GetService<ActivitiesClientConfiguration>();
            return GetElasticConfiguration(config);
        });

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

    private static ElasticConfiguration GetElasticConfiguration(ActivitiesClientConfiguration activitiesdClientConfig)
    {
        var elasticConfig = new ElasticConfiguration()
            .UseSingleNodeConnectionPool(activitiesdClientConfig.ElasticUrl)
            .ScanForIndexMappers(typeof(ActivitiesIndexMapper).Assembly)
            .OnRequestCompleted(r => Log.Debug(r.DebugInformation));

        if (!string.IsNullOrWhiteSpace(activitiesdClientConfig.ElasticUsername) && !string.IsNullOrWhiteSpace(activitiesdClientConfig.ElasticPassword))
        {
            elasticConfig.UseBasicAuthentication(activitiesdClientConfig.ElasticUsername, activitiesdClientConfig.ElasticPassword);
        }

        return elasticConfig;
    }
}