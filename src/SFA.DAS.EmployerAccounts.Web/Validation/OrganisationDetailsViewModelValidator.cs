using FluentValidation;

namespace SFA.DAS.EmployerAccounts.Web.Validation;

public sealed class OrganisationDetailsViewModelValidator : AbstractValidator<OrganisationDetailsViewModel>
{
    public OrganisationDetailsViewModelValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage("Enter a name");
    }
}