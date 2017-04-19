using System.Globalization;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.ViewModels
{
    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel : ViewModelBase
    {
        public ApprenticeshipViewModel()
        {
            StartDate = new DateTimeViewModel();
            EndDate = new DateTimeViewModel();
        }

        public string HashedApprenticeshipId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; } = new DateTimeViewModel(0);

        public string DateOfBirthDayError => GetErrorMessage("DateOfBirth.Day");

        public string NINumber { get; set; }

        public string ApprenticeshipName => $"{FirstName} {LastName}";

        public string ULN { get; set; }

        public TrainingType TrainingType { get; set; }

        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public string Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }

        public string StartMonthName
        {
            get
            {
                return StartDate?.Month != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(StartDate.Month.Value) : string.Empty;
            }
        }

        public DateTimeViewModel EndDate { get; set; }

        public string EndMonthName
        {
            get
            {
                return EndDate?.Month != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(EndDate.Month.Value) : string.Empty;
            }
        }

        public string EmployerRef { get; set; }

        public string ProviderRef { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public AgreementStatus AgreementStatus { get; set; }

        public bool HasStarted { get; set; }

        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
        public string DateOfBirthError => GetErrorMessage(nameof(DateOfBirth));
        public string StartDateError => GetErrorMessage(nameof(StartDate));
        public string EndDateError => GetErrorMessage(nameof(EndDate));
        public string CostError => GetErrorMessage(nameof(Cost));
        public string StartDateOverlapError => GetErrorMessage("StartDateOverlap");
        public string EndDateOverlapError => GetErrorMessage("EndDateOverlap");
        public string EmployerRefError => GetErrorMessage(nameof(EmployerRef));
        public string TrainingCodeError => GetErrorMessage(nameof(TrainingCode));
    }
}