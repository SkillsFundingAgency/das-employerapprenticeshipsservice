using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses
{
    public class GetProvidersResponse
    {
        [JsonProperty("providers")]
        public List<ProviderResponseItem> Providers { get; set; }
    }

    public class ProviderResponseItem
    {
        [JsonProperty("ukprn")]
        public long Ukprn { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contactUrl")]
        public string ContactUrl { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}