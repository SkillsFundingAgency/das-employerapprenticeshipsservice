using System;
using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    public class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            var currentYear = DateTime.Now.Year;

            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Please enter a valid unique learner number");
            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Please enter the total agreed cost");
            RuleFor(x => x.StartMonth).InclusiveBetween(1, 12).WithMessage("Please enter a valid start month for training");
            RuleFor(x => x.StartYear).InclusiveBetween(currentYear, 9999).WithMessage("Please enter a valid start year for training");
            RuleFor(x => x.EndMonth).InclusiveBetween(1, 12).WithMessage("Please enter a valid finish month for training");
            RuleFor(x => x.EndYear).InclusiveBetween(currentYear, 9999).WithMessage("Please enter a valid finish year for training");
        }
    }
}