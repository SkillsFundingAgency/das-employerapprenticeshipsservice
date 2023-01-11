using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class EventsRegistry : Registry
{
    public EventsRegistry()
    {
        For<IEventsApi>()
            .Use<EventsApi>()
            .Ctor<IEventsApiClientConfiguration>()
            .Is(c => c.GetInstance<EmployerAccountsConfiguration>().EventsApi)
            .SelectConstructor(() => new EventsApi(null));
    }
}