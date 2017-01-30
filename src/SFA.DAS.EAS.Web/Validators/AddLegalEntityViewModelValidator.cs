using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using FluentValidation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
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
            var output = 0;
            return int.TryParse(value, out output);
        }
    }
}