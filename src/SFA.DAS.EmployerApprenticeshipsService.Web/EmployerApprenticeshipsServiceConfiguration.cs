namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class EmployerApprenticeshipsServiceConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public EmployerConfiguration Employer { get; set; }
    }

    public class EmployerConfiguration
    {
        public string DatabaseConnectionString  { get; set; }
    }

    public class CompaniesHouseConfiguration
    {
        public string ApiKey { get; set; }  
    }
}