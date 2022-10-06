using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Client.DependencyResolution;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class PaymentsRegistry : PaymentsEventsApiClientRegistry
    {
        public PaymentsRegistry()
        {
            For<PaymentsEventsApiClientLocalConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().PaymentsEventsApi);
            For<IPaymentsEventsApiClientConfiguration>().Use(c => c.GetInstance<PaymentsEventsApiClientLocalConfiguration>());
        }
    }
}