using FluentValidation;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("'ULN' is not in the correct format.");
            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$");
        }
    }
}