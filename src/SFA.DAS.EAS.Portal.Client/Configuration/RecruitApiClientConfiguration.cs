using SFA.DAS.Http;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public class RecruitApiClientConfiguration : IAzureADClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
    }
}
