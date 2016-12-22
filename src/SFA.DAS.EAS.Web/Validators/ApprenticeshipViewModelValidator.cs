using System;
using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    using System.Text.RegularExpressions;
    using Models.Types;

    public class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            var now = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);

            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Enter a valid unique learner number");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Enter a first name");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Enter a last name");

            RuleFor(x => x.NINumber)
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd]$", RegexOptions.IgnoreCase)
                .WithMessage("Enter a valid national insurance number");

            RuleFor(r => r.StartDate)
                .Must(ValidDateValue).Unless(m => m.StartDate == null).WithMessage("Enter a valid start date");

            RuleFor(r => r.EndDate)
                .Must(ValidDateValue).Unless(m => m.EndDate == null).WithMessage("Enter a valid training end date")
                .Must(BeGreaterThanStartDate).WithMessage("The end date must be later than the start date");

            RuleFor(r => r.DateOfBirth)
                .Must(ValidateDateOfBirth).Unless(m => m.DateOfBirth == null).WithMessage("Enter a valid date of birth")
                .Must(m => _checkIfNotNull(m?.DateTime, m?.DateTime < yesterday)).WithMessage("The date of birth must be in the past");

            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Enter the total agreed training cost");
        }

        private bool BeGreaterThanStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        private readonly Func<DateTime?, bool, bool> _checkIfNotNull = (dt, b) => dt == null || b;

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            if (date.DateTime == null)
            {
                if (!date.Day.HasValue && !date.Month.HasValue && !date.Year.HasValue) return true;
                return false;
            }

            if (!date.Day.HasValue || !date.Month.HasValue || !date.Year.HasValue) return false;

            return true;
        }

        private bool ValidDateValue(DateTimeViewModel date)
        {
            if (date.DateTime == null)
            {
                if (!date.Day.HasValue && !date.Month.HasValue && !date.Year.HasValue) return true;
                return false;
            }

            if (!date.Month.HasValue || !date.Year.HasValue) return false;

            return true;
        }
    }
}