using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class SelectEmployerViewModel
    {
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateOfIncorporation { get; set; }

        public string RegisteredAddress { get; set; }
    }
}