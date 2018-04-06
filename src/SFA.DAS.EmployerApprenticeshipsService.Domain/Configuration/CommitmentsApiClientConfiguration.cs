using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration, IConfiguration, IJwtClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
    }
}