using System;
using FluentValidation;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Validation
{
    public class AddLegalEntityViewModelValidator : AbstractValidator<AddLegalEntityViewModel>
    {
        public AddLegalEntityViewModelValidator()
        {
            RuleFor(r => r.OrganisationType).NotEmpty().WithMessage("Select a type of organisation");

            RuleFor(r => r.CompaniesHouseNumber)
                .NotEmpty()
                .When(x => x.OrganisationType == OrganisationType.CompaniesHouse)
                .WithMessage("Enter Companies House number");

            RuleFor(r => r.PublicBodyName)
                .NotEmpty()
                .When(x => x.OrganisationType == OrganisationType.PublicBodies)
                .WithMessage("Enter organisation's name");

            RuleFor(r => r.CharityRegistrationNumber)
                .NotEmpty()
                .When(x => x.OrganisationType == OrganisationType.Charities)
                .WithMessage("Enter charity registration number");

            RuleFor(r => r.CharityRegistrationNumber)
                .Must(CanParseInt32)
                .When(x => x.OrganisationType == OrganisationType.Charities)
                .When(x => !String.IsNullOrWhiteSpace(x.CharityRegistrationNumber))
                .WithMessage("Enter a valid charity registration number");
        }

        private static bool CanParseInt32(string value)
        {
            return int.TryParse(value, out _);
        }
    }
}