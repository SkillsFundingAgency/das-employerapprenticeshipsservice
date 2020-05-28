using SFA.DAS.Provider.Events.Api.Client.Configuration;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class PaymentsApiClientConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
        public bool PaymentsDisabled { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}