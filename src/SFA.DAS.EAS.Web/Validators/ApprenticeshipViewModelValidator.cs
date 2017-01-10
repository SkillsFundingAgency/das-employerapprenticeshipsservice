using System;
using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Models.Types;

    public class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            Func<string, int, bool> lengthLessThan = (str, length) => (str?.Length ?? 0) <= length;
            Func<string, int, bool> numberOfDigitsLessThan = (str, length) => { return (str?.Count(char.IsDigit) ?? 0) < length; };

            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Enter a valid unique learner number");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First names must be entered")
                .Must(m => lengthLessThan(m, 100)).WithMessage("You must enter a first name that's no longer than 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name must be entered")
                .Must(m => lengthLessThan(m, 100)).WithMessage("You must enter a last name that's no longer than 100 characters");

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

            RuleFor(x => x.Cost)
                .Matches("^$|^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$").WithMessage("Enter the total agreed training cost")
                .Must(m => numberOfDigitsLessThan(m, 7)).WithMessage("The cost must be 6 numbers or fewer, for example 25000");

            RuleFor(x => x.EmployerRef)
                .Must(m => lengthLessThan(m, 20)).WithMessage("The reference must be 20 characters or fewer");
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