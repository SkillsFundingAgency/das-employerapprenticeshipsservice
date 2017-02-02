using System;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public sealed class ApprenticeshipListItemViewModel
    {
        public string ApprenticeName { get; set; }

        public string TrainingName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Cost { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public bool CanBeApproved { get; set; }
    }
}