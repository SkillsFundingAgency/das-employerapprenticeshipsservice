using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class CommitmentsApiV2ClientConfiguration : ICommitmentsApiV2ClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }

    public interface ICommitmentsApiV2ClientConfiguration : IAzureActiveDirectoryClientConfiguration
    {
    }
}
