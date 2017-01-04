using System;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class SelectEmployerViewModel
    {
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime? DateOfIncorporation { get; set; }

        public string RegisteredAddress { get; set; }
        public bool HideBreadcrumb { get; set; }
        public string CompanyStatus { get; set; }
        public OrganisationType Source { get; set; }
    }
}