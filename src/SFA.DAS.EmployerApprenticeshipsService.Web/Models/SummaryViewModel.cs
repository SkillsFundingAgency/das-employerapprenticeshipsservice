using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class SummaryViewModel
    {
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateOfIncorporation { get; set; }
        public string RegisteredAddress { get; set; }
        public string EmployerRef { get; set; }
    }
}