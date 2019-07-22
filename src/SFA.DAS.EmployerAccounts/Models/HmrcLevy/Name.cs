using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Models.HmrcLevy
{
    public class Name
    {
        [JsonProperty("nameLine1")]
        public string EmprefAssociatedName { get; set; }
    }
}