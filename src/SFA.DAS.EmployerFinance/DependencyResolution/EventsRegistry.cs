using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class EventsRegistry : Registry
    {
        public EventsRegistry()
        {
            For<IEventsApi>()
                .Use<EventsApi>()
                .Ctor<IEventsApiClientConfiguration>()
                .Is(c => c.GetInstance<EmployerFinanceConfiguration>().EventsApi)
                .SelectConstructor(() => new EventsApi(null));
        }
    }
}