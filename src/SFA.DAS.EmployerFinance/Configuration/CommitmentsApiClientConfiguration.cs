using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Http;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration, IJwtClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}