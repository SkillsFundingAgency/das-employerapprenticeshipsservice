namespace SFA.DAS.EAS.Domain.Configuration
{
    public class PaymentProviderConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
    }
}