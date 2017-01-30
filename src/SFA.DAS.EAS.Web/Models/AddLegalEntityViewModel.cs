using FluentValidation.Attributes;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Models
{
    [Validator(typeof(AddLegalEntityViewModelValidator))]
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