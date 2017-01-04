using System.Globalization;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Models.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Models
{
    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public sealed class ApprenticeshipViewModel
    {
        public string HashedApprenticeshipId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; } = new DateTimeViewModel(0);

        public string NINumber { get; set; }

        public string ApprenticeshipName => $"{FirstName} {LastName}";

        public string ULN { get; set; }

        public TrainingType TrainingType { get; set; }

        public string TrainingId { get; set; }
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
    }
}