using System;

namespace SFA.DAS.EAS.Web.Models
{
    public class OrganisationDetailsViewModel : NavigationViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfInception { get; set; }
    }
}