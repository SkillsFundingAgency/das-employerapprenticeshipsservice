using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : IConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public SmtpConfiguration SmtpServer { get; set; }
        public string DashboardUrl { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public string DatabaseConnectionString { get; set; }
    }
}