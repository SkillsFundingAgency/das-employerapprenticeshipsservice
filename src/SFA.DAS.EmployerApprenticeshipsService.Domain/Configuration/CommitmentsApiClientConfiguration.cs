using SFA.DAS.Commitments.Api.Client.Configuration;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration
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