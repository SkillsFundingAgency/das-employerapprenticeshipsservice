using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class LevyDeclarationProviderConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}