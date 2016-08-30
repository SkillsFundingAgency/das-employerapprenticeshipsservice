namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class HmrcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
        public string ServerToken { get; set; }

        public bool IgnoreDuplicates { get; set; }
    }
}