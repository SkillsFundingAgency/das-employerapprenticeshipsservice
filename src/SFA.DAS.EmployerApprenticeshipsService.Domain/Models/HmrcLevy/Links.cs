using Newtonsoft.Json;

namespace SFA.DAS.EAS.Domain.Models.HmrcLevy
{
    public class Links
    {
        [JsonProperty("self")]
        public Link Self { get; set; }
        [JsonProperty("declarations")]
        public Link Declarations { get; set; }
        [JsonProperty("fractions")]
        public Link Fractions { get; set; }
    }
}