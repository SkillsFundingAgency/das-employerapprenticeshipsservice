using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Provider
    {
        [JsonProperty("ukprn")]
        public long Ukprn { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("town")]
        public string Town { get; set; }
        [JsonProperty("postcode")]
        public string Postcode { get; set; }
    }
}