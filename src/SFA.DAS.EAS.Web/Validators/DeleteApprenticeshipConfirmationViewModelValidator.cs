using System;
using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    public class DeleteApprenticeshipConfirmationViewModelValidator : AbstractValidator<DeleteApprenticeshipConfirmationViewModel>
    {
        public DeleteApprenticeshipConfirmationViewModelValidator()
        {
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Please choose an option");
        }
    }
}