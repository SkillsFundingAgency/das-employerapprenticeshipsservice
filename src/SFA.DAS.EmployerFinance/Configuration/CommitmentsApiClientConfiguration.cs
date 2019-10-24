using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration, IAzureActiveDirectoryClientConfiguration, IJwtClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}