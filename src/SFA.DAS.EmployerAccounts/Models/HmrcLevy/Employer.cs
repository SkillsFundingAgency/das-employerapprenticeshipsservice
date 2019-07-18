using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Models.HmrcLevy
{
    public class Employer
    {
        [JsonProperty("name")]
        public Name Name { get; set; }
    }
}