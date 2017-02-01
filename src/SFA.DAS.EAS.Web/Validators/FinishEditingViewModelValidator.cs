using FluentValidation;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class FinishEditingViewModelValidator : AbstractValidator<FinishEditingViewModel>
    {
        public FinishEditingViewModelValidator()
        {
            RuleFor(x => x.SaveStatus).IsInEnum().WithMessage("Select an option");
        }
    }
}