using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public EmployerConfiguration Employer { get; set; }
        public string  ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public SmtpConfiguration SmtpServer { get; set; }

        public string DashboardUrl { get; set; }

        public HmrcConfiguration Hmrc { get; set; }
    }

    public class HmrcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }

        public bool DuplicatesCheck { get; set; }
    }

    public class IdentityServerConfiguration
    {
        public bool UseFake { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
    }

    public class EmployerConfiguration
    {
        public string DatabaseConnectionString { get; set; }
    }

    public class CompaniesHouseConfiguration
    {
        public string ApiKey { get; set; }
    }
   
    public class SmtpConfiguration
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }
}