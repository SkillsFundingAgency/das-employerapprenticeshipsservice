using System.Globalization;
using FluentValidation.Attributes;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Models
{
    [Validator(typeof(ApprenticeshipViewModelValidator))]
    public class ApprenticeshipViewModel
    {
        public string HashedId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string HashedAccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApprenticeshipName => $"{FirstName} {LastName}";
        public string ULN { get; set; }
        public TrainingType TrainingType { get; set; } // TODO: LWA Don't think we need these
        public string TrainingId { get; set; }
        public string TrainingName { get; set; } // TODO: LWA Don't think we need these
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
        public ApprenticeshipStatus Status { get; set; }
        public string AgreementStatus { get; set; }
    }
}