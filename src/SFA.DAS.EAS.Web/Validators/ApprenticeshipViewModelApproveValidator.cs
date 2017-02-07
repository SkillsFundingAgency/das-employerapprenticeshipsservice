using FluentValidation;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class ApprenticeshipViewModelApproveValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelApproveValidator()
        {
            RuleFor(r => r.FirstName).NotEmpty();

            RuleFor(r => r.LastName).NotEmpty();

            RuleFor(r => r.Cost).NotEmpty();

            RuleFor(r => r.StartDate)
                .Must(m => m?.DateTime != null);

            RuleFor(r => r.EndDate)
                .Must(m => m?.DateTime != null);

            RuleFor(r => r.TrainingId).NotEmpty();

            RuleFor(r => r.DateOfBirth)
                .Must(m => m?.DateTime != null);

            RuleFor(r => r.NINumber).NotEmpty();
        }
    }
}