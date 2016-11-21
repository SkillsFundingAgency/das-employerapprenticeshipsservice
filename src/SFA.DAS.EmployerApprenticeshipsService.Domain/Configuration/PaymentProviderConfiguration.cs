using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class PaymentProviderConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}