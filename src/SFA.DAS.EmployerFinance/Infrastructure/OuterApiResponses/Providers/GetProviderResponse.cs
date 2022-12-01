using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Providers
{
    public class GetProviderResponse : ProviderResponse
    {
    }

    public class ProviderResponse
    {
        [JsonProperty("ukprn")]
        public int Ukprn { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
