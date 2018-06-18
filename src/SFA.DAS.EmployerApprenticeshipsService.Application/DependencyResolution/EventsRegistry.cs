using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class EventsRegistry : Registry
    {
        public EventsRegistry()
        {
            For<IEventsApi>()
                .Use<EventsApi>()
                .Ctor<IEventsApiClientConfiguration>()
                .Is(c => c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().EventsApi)
                .SelectConstructor(() => new EventsApi(null));
        }
    }
}