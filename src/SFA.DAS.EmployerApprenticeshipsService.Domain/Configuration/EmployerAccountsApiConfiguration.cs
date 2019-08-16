
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Domain.Configuration
{
    //todo: own interface??
    public class EmployerAccountsApiConfiguration : IAzureADClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string TimeoutTimeSpan { get; set; }
    }
}
