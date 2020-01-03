using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class TokenServiceApiClientConfiguration : ITokenServiceApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
    }
}