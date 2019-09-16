using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EAS.Portal.Configuration
{
    public class CommitmentsApiClientConfiguration : IJwtClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}