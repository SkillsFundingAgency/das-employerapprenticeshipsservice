using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class TokenServiceApiClientConfiguration : ITokenServiceApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
    }
}