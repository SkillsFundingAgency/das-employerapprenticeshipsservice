using System;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class AddOrganisationAddressModel : ViewModelBase
    {
        public string AddressFirstLine { get; set; }
        public string AddressSecondLine { get; set; }
        public string TownOrCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }

        public string OrganisationHashedId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationReferenceNumber { get; set; }
        public DateTime? OrganisationDateOfInception { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public short? PublicSectorDataSource { get; set; }
        public string OrganisationStatus { get; set; }

        public string AddressFirstLineError => GetErrorMessage(nameof(AddressFirstLine));
        public string TownOrCityError => GetErrorMessage(nameof(TownOrCity));
        public string PostcodeError => GetErrorMessage(nameof(Postcode));

       

    }
}