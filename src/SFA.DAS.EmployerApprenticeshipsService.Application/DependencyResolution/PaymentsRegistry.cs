using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Provider.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class PaymentsRegistry : Registry
    {
        public PaymentsRegistry()
        {
            For<IPaymentsEventsApiConfiguration>().Use(c => c.GetInstance<PaymentsApiClientConfiguration>());
            For<PaymentsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<PaymentsApiClientConfiguration>(ConfigurationKeys.PaymentsApiClient)).Singleton();
        }
    }
}