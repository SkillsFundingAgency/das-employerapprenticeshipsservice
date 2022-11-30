using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.TrainingCourses
{
    public class GetStandardsResponse
    {
        [JsonProperty("standards")]
        public List<StandardResponse> Standards { get; set; }
    }

    public class StandardResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("maxFunding")]
        public int MaxFunding { get; set; }
    }
}