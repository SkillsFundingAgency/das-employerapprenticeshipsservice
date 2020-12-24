using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses
{
    public class GetFrameworksResponse
    {
        [JsonProperty("frameworks")]
        public List<FrameworkResponseItem> Frameworks { get; set; }
    }

    public class FrameworkResponseItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("frameworkName")]
        public string FrameworkName { get; set; }

        [JsonProperty("pathwayName")]
        public string PathwayName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("frameworkCode")]
        public long FrameworkCode { get; set; }

        [JsonProperty("progType")]
        public long ProgType { get; set; }

        [JsonProperty("pathwayCode")]
        public long PathwayCode { get; set; }

        [JsonProperty("maxFunding")]
        public long MaxFunding { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }       
    }
}