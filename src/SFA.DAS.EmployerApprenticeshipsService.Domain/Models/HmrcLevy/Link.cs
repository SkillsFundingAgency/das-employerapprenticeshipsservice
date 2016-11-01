using Newtonsoft.Json;

namespace SFA.DAS.EAS.Domain.Models.HmrcLevy
{
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}