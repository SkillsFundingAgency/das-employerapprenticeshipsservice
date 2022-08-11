using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.TrainingCourses
{
    public class GetFrameworksResponse
    {
        [JsonProperty("frameworks")]
        public List<FrameworkResponse> Frameworks { get; set; }
    }

    public class FrameworkResponse
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
        public int Level { get; set; }

        [JsonProperty("frameworkCode")]
        public int FrameworkCode { get; set; }

        [JsonProperty("progType")]
        public int ProgType { get; set; }

        [JsonProperty("pathwayCode")]
        public int PathwayCode { get; set; }

        [JsonProperty("maxFunding")]
        public int MaxFunding { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }       
    }
}