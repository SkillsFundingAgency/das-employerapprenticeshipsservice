using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public class RecruitApiClientConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string TimeoutTimeSpan { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}
