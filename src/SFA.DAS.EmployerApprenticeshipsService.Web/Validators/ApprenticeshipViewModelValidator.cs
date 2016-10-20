using FluentValidation;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Validators
{
    public class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Please enter the unique learner number - this should be 10 digits long.");
            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Please enter the cost in whole pounds. For example, for £1500 you should enter 1500.");
        }
    }
}