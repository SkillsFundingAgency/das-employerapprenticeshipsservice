using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class EventsApiClientConfiguration : IEventsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}