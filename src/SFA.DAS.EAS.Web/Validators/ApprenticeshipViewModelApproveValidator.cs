using FluentValidation;

using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    public class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator()
        {

            RuleFor(r => r.FirstName).NotEmpty();
            RuleFor(r => r.LastName).NotEmpty();
            RuleFor(r => r.Cost).NotEmpty();

            RuleFor(r => r.StartMonth).NotEmpty();
            RuleFor(r => r.StartYear).NotEmpty();

            RuleFor(r => r.EndMonth).NotEmpty();
            RuleFor(r => r.EndYear).NotEmpty();

            RuleFor(r => r.TrainingId).NotEmpty();

            RuleFor(r => r.DateOfBirthDay).NotEmpty();
            RuleFor(r => r.DateOfBirthMonth).NotEmpty();
            RuleFor(r => r.DateOfBirthYear).NotEmpty();

            RuleFor(r => r.NINumber).NotEmpty();
        }
    }
}