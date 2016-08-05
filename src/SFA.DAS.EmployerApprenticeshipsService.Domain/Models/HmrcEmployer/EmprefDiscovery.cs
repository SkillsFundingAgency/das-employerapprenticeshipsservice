using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcEmployer
{
    public class EmprefDiscovery
    {
        [JsonProperty("emprefs")]
        public List<string> Emprefs { get; set; }
    }
}