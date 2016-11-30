using System;

namespace SFA.DAS.EAS.Web.Models
{
    public class SummaryViewModel
    {
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateOfIncorporation { get; set; }
        public string RegisteredAddress { get; set; }
        public string EmployerRef { get; set; }
        public bool EmpRefNotFound { get; set; }
        public bool HideBreadcrumb { get; set; }
    }
}