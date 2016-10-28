using Newtonsoft.Json;

namespace SFA.DAS.EAS.Domain.Models.HmrcLevy
{
    public class Name
    {
        [JsonProperty("nameLine1")]
        public string EmprefAssociatedName { get; set; }
    }
}