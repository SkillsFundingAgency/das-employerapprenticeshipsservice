using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class EventsApiServiceRegistrations
{
    public static IServiceCollection AddEventsApi(this IServiceCollection services)
    {
        services.AddSingleton<IEventsApiClientConfiguration>(cfg => cfg.GetService<EmployerAccountsConfiguration>().EventsApi);

        services.AddTransient<IEventsApi>(s =>
        {
            var config = s.GetService<IEventsApiClientConfiguration>();
            return new EventsApi(config);
        });
        
        return services;
    }
}