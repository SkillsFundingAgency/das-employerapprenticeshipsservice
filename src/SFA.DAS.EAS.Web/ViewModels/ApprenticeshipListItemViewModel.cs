using System;
using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public sealed class ApprenticeshipListItemViewModel
    {
        public string ApprenticeName { get; set; }
        public DateTime? ApprenticeDateOfBirth { get; set; }

        public string TrainingCode { get; set; }

        public string TrainingName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? Cost { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public bool CanBeApproved { get; set; }

        public IEnumerable<OverlappingApprenticeship> OverlappingApprenticeships { get; set; }
    }
}