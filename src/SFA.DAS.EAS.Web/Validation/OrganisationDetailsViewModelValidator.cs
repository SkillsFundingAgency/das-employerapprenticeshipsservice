using FluentValidation;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Validation
{
    public sealed class OrganisationDetailsViewModelValidator : AbstractValidator<OrganisationDetailsViewModel>
    {
        public OrganisationDetailsViewModelValidator()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("Enter a name");
        }
    }
}