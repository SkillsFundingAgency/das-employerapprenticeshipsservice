using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Interfaces
{
    public interface IEmployerVerificationService
    {
        Task<EmployerInformation> GetInformation(string id);
    }

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