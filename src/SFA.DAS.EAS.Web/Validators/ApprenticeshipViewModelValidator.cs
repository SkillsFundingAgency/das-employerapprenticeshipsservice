using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Validators.Messages;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public class ApprenticeshipViewModelValidator : ApprenticeshipCoreValidator
    {
        public ApprenticeshipViewModelValidator() : this(new WebApprenticeshipValidationText(), new CurrentDateTime(), new AcademicYear(new CurrentDateTime()))
        { } // The default is used by the MVC model binding

        public ApprenticeshipViewModelValidator(WebApprenticeshipValidationText validationText, ICurrentDateTime currentDateTime, IAcademicYear academicYear) : base(validationText, currentDateTime, academicYear)
        {
        }

        protected override void ValidateUln()
        {
            When(x => !string.IsNullOrEmpty(x.ULN), () =>
            {
                base.ValidateUln();
            });
        }

        protected override void ValidateTraining()
        {
            When(x => !string.IsNullOrEmpty(x.TrainingCode), () =>
            {
                base.ValidateTraining();
            });
        }

        protected override void ValidateDateOfBirth()
        {
            When(x => HasAnyValuesSet(x.DateOfBirth), () =>
            {
                base.ValidateDateOfBirth();
            });
        }

        protected override void ValidateStartDate()
        {
            When(x => HasYearOrMonthValueSet(x.StartDate), () =>
            {
                base.ValidateStartDate();
            });

        }

        protected override void ValidateEndDate()
        {
            When(x => HasYearOrMonthValueSet(x.EndDate), () =>
            {
                base.ValidateEndDate();
            });
        }

        protected override void ValidateCost()
        {
            When(x => !string.IsNullOrEmpty(x.Cost), () =>
            {
                base.ValidateCost();
            });
        }

        private bool HasYearOrMonthValueSet(DateTimeViewModel date)
        {
            if (date == null) return false;

            if (date.Day.HasValue || date.Month.HasValue || date.Year.HasValue) return true;

            return false;
        }

        private bool HasAnyValuesSet(DateTimeViewModel dateOfBirth)
        {
            if (dateOfBirth == null) return false;

            if (dateOfBirth.Day.HasValue || dateOfBirth.Month.HasValue || dateOfBirth.Year.HasValue) return true;

            return false;
        }
    }
}