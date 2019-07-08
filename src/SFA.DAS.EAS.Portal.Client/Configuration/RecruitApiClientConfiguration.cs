using SFA.DAS.Http;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public class RecruitApiClientConfiguration : IJwtClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}
