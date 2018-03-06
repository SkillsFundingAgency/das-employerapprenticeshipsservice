using FluentValidation.Attributes;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Web.Validation;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
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