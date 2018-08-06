using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Provider.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class PaymentsRegistry : Registry
    {
        public PaymentsRegistry()
        {
            For<IPaymentsEventsApiConfiguration>().Use(c => c.GetInstance<PaymentsApiClientConfiguration>());
            For<PaymentsApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<PaymentsApiClientConfiguration>("SFA.DAS.PaymentsAPI")).Singleton();
        }
    }
}