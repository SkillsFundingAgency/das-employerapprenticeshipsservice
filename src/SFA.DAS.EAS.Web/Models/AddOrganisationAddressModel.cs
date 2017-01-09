using System;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class AddOrganisationAddressModel
    {
        public string AddressFirstLine { get; set; }
        public string AddressSecondLine { get; set; }
        public string TownOrCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }

        public string OrganisationHashedId { get; set; }
        public string OrganisationName { get; set; }
        public string OrgainsationReferenceNumber { get; set; }
        public DateTime? OrgainsationDateOfInception { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public string OrganisationStatus { get; set; }
       
    }
}