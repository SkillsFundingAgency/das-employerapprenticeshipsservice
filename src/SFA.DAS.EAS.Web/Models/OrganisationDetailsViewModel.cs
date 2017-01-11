using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class OrganisationDetailsViewModel : NavigationViewModel
    {
        public string HashedId { get; set; }
        public OrganisationType Type { get; set; }
        [AllowHtml]
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfInception { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public bool AddedToAccount { get; set; }
        public string NameError => GetErrorMessage(nameof(Name));
    }
}