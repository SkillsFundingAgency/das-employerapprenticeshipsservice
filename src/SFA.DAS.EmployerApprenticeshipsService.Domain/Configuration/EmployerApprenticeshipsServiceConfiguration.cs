using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public EmployerConfiguration Employer { get; set; }
        public string  ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
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
    
    public static class QueueNames
    {
        public static string das_at_eas_refresh_employer_levy { get; set; }
        public static string das_at_eas_get_employer_levy  { get; set; }
        
    }
}