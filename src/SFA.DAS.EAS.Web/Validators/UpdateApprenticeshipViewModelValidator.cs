using FluentValidation;

using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class UpdateApprenticeshipViewModelValidator : AbstractValidator<UpdateApprenticeshipViewModel>
    {
        public UpdateApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotNull().WithMessage("Select an option");
        }
    }
}