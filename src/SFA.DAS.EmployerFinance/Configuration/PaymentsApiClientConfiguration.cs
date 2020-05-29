using SFA.DAS.Provider.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class PaymentsApiClientConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public bool PaymentsDisabled { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}