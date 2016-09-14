using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class TasksApiConfiguration : IConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientSecret { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
