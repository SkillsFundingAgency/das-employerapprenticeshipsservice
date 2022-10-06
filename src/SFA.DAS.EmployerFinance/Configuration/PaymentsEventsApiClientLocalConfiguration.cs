using SFA.DAS.Provider.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class PaymentsEventsApiClientLocalConfiguration : PaymentsEventsApiClientConfiguration, IPaymentsEventsApiClientConfiguration
    {
        public bool PaymentsDisabled { get; set; }
    }
}