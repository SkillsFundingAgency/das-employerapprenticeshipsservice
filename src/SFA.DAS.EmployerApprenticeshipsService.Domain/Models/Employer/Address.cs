using Newtonsoft.Json;

namespace SFA.DAS.EAS.Domain.Models.Employer
{
    public class Address
    {
        [JsonProperty("address_line_1")]
        public string Line1 { get; set; }
        [JsonProperty("address_line_2")]
        public string Line2 { get; set; }
        [JsonProperty("locality")]
        public string TownOrCity { get; set; }
        [JsonProperty("region")]
        public string County { get; set; }
        [JsonProperty("postal_code")]
        public string PostCode { get; set; }
    }
}