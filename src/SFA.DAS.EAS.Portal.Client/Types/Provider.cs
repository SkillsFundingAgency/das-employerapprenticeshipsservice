using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Provider
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("ukprn")]
        public long Ukprn { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("telephone")]
        public string Telephone { get; set; }
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }
    }
}