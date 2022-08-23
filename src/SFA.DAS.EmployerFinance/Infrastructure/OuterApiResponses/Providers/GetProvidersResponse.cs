using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Providers
{
    public class GetProvidersResponse
    {
        [JsonProperty("providers")]
        public List<ProviderResponse> Providers { get; set; }
    }
}