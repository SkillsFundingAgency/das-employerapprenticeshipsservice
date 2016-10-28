using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : IConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public string DatabaseConnectionString { get; set; }
        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public TasksApiClientConfiguration TasksApi { get; set; }
        public string Hashstring { get; set; }
        public List<EmailTemplateConfigurationItem> EmailTemplates { get; set; }
    }
}