using System;
using System.Globalization;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel
    {
        public string HashedId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DateOfBirth { get; set; }

        public string NINumber { get; set; }
        public string ApprenticeshipName => $"{FirstName} {LastName}";
        public string ULN { get; set; }
        public TrainingType TrainingType { get; set; }
        public string TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string Cost { get; set; }

        public int? StartMonth { get; set; }

        public string StartMonthName
        {
            get
            {
                return StartMonth.HasValue ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(StartMonth.Value) : string.Empty;
            }
        }

        public int? StartYear { get; set; }
        public int? EndMonth { get; set; }

        public string EndMonthName
        {
            get
            {
                return EndMonth.HasValue ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(EndMonth.Value) : string.Empty;
            }
        }

        public int? EndYear { get; set; }

        public string EmployerRef { get; set; }

        public string ProviderRef { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public AgreementStatus AgreementStatus { get; set; }
    }
}