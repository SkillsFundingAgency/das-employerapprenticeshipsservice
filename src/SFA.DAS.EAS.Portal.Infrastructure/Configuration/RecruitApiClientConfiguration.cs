using SFA.DAS.Http;

namespace SFA.DAS.EAS.Portal.Infrastructure.Configuration
{
    public class RecruitApiClientConfiguration: IJwtClientConfiguration
    {
        public string ApiBaseUrl { get; }
        public string ClientToken { get; }
    }
}
