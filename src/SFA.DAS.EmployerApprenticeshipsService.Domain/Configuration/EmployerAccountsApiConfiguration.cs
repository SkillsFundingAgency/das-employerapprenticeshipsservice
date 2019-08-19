
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerAccountsApiConfiguration : IAzureADClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string TimeoutTimeSpan { get; set; }
    }
}
