using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Models.HmrcLevy
{
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}