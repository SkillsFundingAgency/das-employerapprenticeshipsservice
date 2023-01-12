using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.Web;

public static class EventsApiRegistrationExtensions
{
    public static void AddEventsApi(this IServiceCollection services)
    {
        services.AddTransient<IEventsApi>(s =>
        {
            var config = s.GetService<IEventsApiClientConfiguration>();
            return new EventsApi(config);
        });

        services.AddTransient<IEventsApi>(_ => new EventsApi(null));
        
        //For<IEventsApi>()
        //    .Use<EventsApi>()
        //    .Ctor<IEventsApiClientConfiguration>()
        //    .Is(c => c.GetInstance<EmployerAccountsConfiguration>().EventsApi)
        //    .SelectConstructor(() => new EventsApi(null));
    }
}