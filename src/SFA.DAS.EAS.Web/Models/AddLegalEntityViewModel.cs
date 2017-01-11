using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class AddLegalEntityViewModel: ViewModelBase
    {
        public string HashedAccountId { get; set; }

        public OrganisationType? OrganisationType { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public string PublicBodyName { get; set; }
        public string CharityRegistrationNumber { get; set; }

        public string OrganisationTypeError => GetErrorMessage("OrganisationType");
        public string CompaniesHouseNumberError => GetErrorMessage("CompaniesHouseNumber");
        public string PublicBodyNameError => GetErrorMessage("PublicBodyName");
        public string CharityRegistrationNumberError => GetErrorMessage("CharityRegistrationNumber");
    }
}