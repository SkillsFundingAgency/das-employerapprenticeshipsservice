using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy
{
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}