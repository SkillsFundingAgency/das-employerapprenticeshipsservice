using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy
{
    public class Name
    {
        [JsonProperty("nameLine1")]
        public string EmprefAssociatedName { get; set; }
    }
}