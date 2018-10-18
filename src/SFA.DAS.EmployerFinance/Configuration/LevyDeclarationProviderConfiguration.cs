namespace SFA.DAS.EmployerFinance.Configuration
{
    public class LevyDeclarationProviderConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
        public decimal TransferAllowancePercentage { get; set; }
    }
}