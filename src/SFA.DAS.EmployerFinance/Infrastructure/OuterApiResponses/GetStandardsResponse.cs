using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses
{
    public class GetStandardsResponse
    {
        [JsonProperty("standards")]
        public List<StandardResponseItem> Standards { get; set; }
    }

    public class StandardResponseItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("maxFunding")]
        public long MaxFunding { get; set; }
    }
}