using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Provider.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
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