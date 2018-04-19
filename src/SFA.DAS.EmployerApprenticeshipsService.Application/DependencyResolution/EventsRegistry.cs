using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class EventsRegistry : Registry
    {
        public EventsRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName);

            For<IEventsApi>().Use<EventsApi>().Ctor<IEventsApiClientConfiguration>().Is(config.EventsApi).SelectConstructor(() => new EventsApi(null));
        }
    }
}