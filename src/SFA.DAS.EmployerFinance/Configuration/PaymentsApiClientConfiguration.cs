using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class PaymentsApiClientConfiguration : IPaymentsEventsApiConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public bool PaymentsDisabled { get; set; }
    }
}