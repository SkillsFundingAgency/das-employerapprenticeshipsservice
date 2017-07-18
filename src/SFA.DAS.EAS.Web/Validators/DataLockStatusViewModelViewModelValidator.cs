using FluentValidation;

using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Validators
{
    public sealed class DataLockStatusViewModelViewModelValidator : AbstractValidator<DataLockStatusViewModel>
    {
        public DataLockStatusViewModelViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotNull().WithMessage("Select an option");
        }
    }
}