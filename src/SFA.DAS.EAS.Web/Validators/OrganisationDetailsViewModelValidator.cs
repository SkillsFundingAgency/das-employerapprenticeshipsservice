using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class OrganisationDetailsViewModelValidator : AbstractValidator<OrganisationDetailsViewModel>
    {
        public OrganisationDetailsViewModelValidator()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("Enter a name");
        }
    }
}