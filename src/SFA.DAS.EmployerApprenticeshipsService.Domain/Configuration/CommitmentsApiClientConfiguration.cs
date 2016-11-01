using SFA.DAS.Commitments.Api.Client.Configuration;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}