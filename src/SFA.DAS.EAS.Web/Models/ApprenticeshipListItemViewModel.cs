namespace SFA.DAS.EAS.Web.Models
{
    using System;

    public sealed class ApprenticeshipListItemViewModel
    {
        public string ApprenticeshipName { get; set; }

        public string TrainingName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Cost { get; set; }

        public string HashedId { get; set; }
    }
}