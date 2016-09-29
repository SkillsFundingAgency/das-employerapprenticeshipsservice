using System.Globalization;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class ApprenticeshipViewModel
    {
        public long Id { get; set; }
        public long CommitmentId { get; set; }
        public long AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ULN { get; set; }
        public string TrainingId { get; set; } //standard or framework
        public decimal? Cost { get; set; }
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
        public string Status { get; set; }
        public string AgreementStatus { get; set; }

    }
}