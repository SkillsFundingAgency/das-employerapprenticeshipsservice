using System;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class EmployerInformation
    {
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("company_number")]
        public string CompanyNumber { get; set; }

        [JsonProperty("date_of_creation")]
        public DateTime DateOfIncorporation { get; set; }
    }
}