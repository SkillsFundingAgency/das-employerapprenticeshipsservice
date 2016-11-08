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

            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Please enter the unique learner number - this should be 10 digits long");
            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Please enter the cost in whole pounds. For example, for £1500 you should enter 1500");
            RuleFor(x => x.StartMonth).InclusiveBetween(1, 12).WithMessage("Please enter a valid start month");
            RuleFor(x => x.StartYear).InclusiveBetween(currentYear, 9999).WithMessage("Please enter a valid start year");
            RuleFor(x => x.EndMonth).InclusiveBetween(1, 12).WithMessage("Please enter a valid end month.");
            RuleFor(x => x.EndYear).InclusiveBetween(currentYear, 9999).WithMessage("Please enter a valid end year");
        }
    }
}