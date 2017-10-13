using FluentValidation;

using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class DeleteCohortConfirmationViewModelValidator : AbstractValidator<DeleteCommitmentViewModel>
    {
        public DeleteCohortConfirmationViewModelValidator()
        {
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Confirm deletion");
        }
    }
}