using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.Configuration;

public class EventsApiClientConfiguration : IEventsApiClientConfiguration
{
    public string BaseUrl { get; set; }
    public string ClientToken { get; set; }
}