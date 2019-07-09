using SFA.DAS.Http;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public class RecruitApiClientConfiguration : IAzureADClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}
