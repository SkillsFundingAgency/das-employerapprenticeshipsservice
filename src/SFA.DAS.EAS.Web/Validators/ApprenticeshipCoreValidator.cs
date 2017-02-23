using System;
using System.Linq;
using FluentValidation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Validators.Messages;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public class ApprenticeshipCoreValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        protected static readonly Func<string, int, bool> LengthLessThanFunc = (str, length) => (str?.Length ?? length) < length;
        protected static readonly Func<DateTime?, bool, bool> CheckIfNotNull = (dt, b) => dt == null || b;
        protected static readonly Func<string, int, bool> HaveNumberOfDigitsFewerThan = (str, length) => { return (str?.Count(char.IsDigit) ?? 0) < length; };
        private readonly IApprenticeshipValidationErrorText _validationText;
        private readonly ICurrentDateTime _currentDateTime;

        public ApprenticeshipCoreValidator(IApprenticeshipValidationErrorText validationText, ICurrentDateTime currentDateTime)
        {
            _validationText = validationText;
            _currentDateTime = currentDateTime;

            ValidateFirstName();

            ValidateLastName();

            ValidateUln();

            ValidateTraining();

            ValidateDateOfBirth();

            ValidateStartDate();

            ValidateEndDate();

            ValidateCost();

            ValidateEmployerReference();
        }

        private void ValidateFirstName()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(_validationText.GivenNames01.Text).WithErrorCode(_validationText.GivenNames01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(_validationText.GivenNames02.Text).WithErrorCode(_validationText.GivenNames02.ErrorCode);
        }

        private void ValidateLastName()
        {
            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(_validationText.FamilyName01.Text).WithErrorCode(_validationText.FamilyName01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(_validationText.FamilyName02.Text).WithErrorCode(_validationText.FamilyName02.ErrorCode); ;
        }

        protected virtual void ValidateUln()
        {
            RuleFor(x => x.ULN)
                .NotNull().WithMessage(_validationText.Uln01.Text).WithErrorCode(_validationText.Uln01.ErrorCode)
                .Matches("^[1-9]{1}[0-9]{9}$").WithMessage(_validationText.Uln01.Text).WithErrorCode(_validationText.Uln01.ErrorCode)
                .Must(m => m != "9999999999").WithMessage(_validationText.Uln02.Text).WithErrorCode(_validationText.Uln02.ErrorCode);
        }

        protected virtual void ValidateTraining()
        {
            RuleFor(x => x.TrainingCode)
                .NotEmpty().WithMessage(_validationText.TrainingCode01.Text).WithErrorCode(_validationText.TrainingCode01.ErrorCode); ;
        }

        protected virtual void ValidateDateOfBirth()
        {
            RuleFor(r => r.DateOfBirth)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.DateOfBirth01.Text).WithErrorCode(_validationText.DateOfBirth01.ErrorCode)
                .Must(ValidateDateOfBirth).WithMessage(_validationText.DateOfBirth01.Text).WithErrorCode(_validationText.DateOfBirth01.ErrorCode)
                .Must((apprenticship, dob) =>
                {
                    return WillApprenticeBeAtLeast15AtStartOfTraining(apprenticship, dob);
                }).WithMessage(_validationText.DateOfBirth02.Text).WithErrorCode(_validationText.DateOfBirth02.ErrorCode);
        }

        protected virtual void ValidateStartDate()
        {
            RuleFor(x => x.StartDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.LearnStartDate01.Text).WithErrorCode(_validationText.LearnStartDate01.ErrorCode)
                .Must(ValidateDateWithoutDay).WithMessage(_validationText.LearnStartDate01.Text).WithErrorCode(_validationText.LearnStartDate01.ErrorCode)
                .Must(NotBeBeforeMay2017).WithMessage(_validationText.LearnStartDate02.Text).WithErrorCode(_validationText.LearnStartDate02.ErrorCode);
        }

        protected virtual void ValidateEndDate()
        {
            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.LearnPlanEndDate01.Text).WithErrorCode(_validationText.LearnPlanEndDate01.ErrorCode)
                .Must(ValidateDateWithoutDay).WithMessage(_validationText.LearnPlanEndDate01.Text).WithErrorCode(_validationText.LearnPlanEndDate01.ErrorCode)
                .Must(BeGreaterThenStartDate).WithMessage(_validationText.LearnPlanEndDate02.Text).WithErrorCode(_validationText.LearnPlanEndDate02.ErrorCode)
                .Must(m => m.DateTime > _currentDateTime.Now).WithMessage(_validationText.LearnPlanEndDate03.Text).WithErrorCode(_validationText.LearnPlanEndDate03.ErrorCode);
        }

        protected virtual void ValidateCost()
        {
            RuleFor(x => x.Cost)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(_validationText.TrainingPrice01.Text).WithErrorCode(_validationText.TrainingPrice01.ErrorCode)
                .Matches("^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$|^[1-9]{1}[0-9]*$").WithMessage(_validationText.TrainingPrice01.Text).WithErrorCode(_validationText.TrainingPrice01.ErrorCode)
                .Must(m => decimal.Parse(m) <= 100000).WithMessage(_validationText.TrainingPrice02.Text).WithErrorCode(_validationText.TrainingPrice02.ErrorCode);
        }

        private void ValidateEmployerReference()
        {
            RuleFor(x => x.EmployerRef)
                .Must(m => LengthLessThanFunc(m, 21))
                    .When(x => !string.IsNullOrEmpty(x.ProviderRef)).WithMessage(_validationText.EmployerRef01.Text).WithErrorCode(_validationText.EmployerRef01.ErrorCode);
        }

        private bool WillApprenticeBeAtLeast15AtStartOfTraining(ApprenticeshipViewModel model, DateTimeViewModel dob)
        {
            DateTime? startDate = model?.StartDate?.DateTime;
            DateTime? dobDate = dob?.DateTime;

            if (startDate == null || dob == null) return true; // Don't fail validation if both fields not set

            int age = startDate.Value.Year - dobDate.Value.Year;
            if (startDate < dobDate.Value.AddYears(age)) age--;

            return age >= 15;
        }

        private bool BeGreaterThenStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        private bool ValidateDateWithoutDay(DateTimeViewModel date)
        {
            return date.DateTime != null;
        }

        private bool NotBeBeforeMay2017(DateTimeViewModel date)
        {
            return date.DateTime >= new DateTime(2017, 5, 1);
        }

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            // Check the day has value as the view model supports just month and year entry
            if (date.DateTime == null || !date.Day.HasValue) return false;

            return true;
        }
    }
}