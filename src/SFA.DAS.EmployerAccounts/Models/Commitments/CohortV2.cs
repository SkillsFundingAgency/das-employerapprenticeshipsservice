using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Models.Commitments
{
    public class CohortV2
    {
        public long CohortId { get; set; }
        public int? CohortsCount { get; set; }
        public int? NumberOfDraftApprentices { get; set; }        
        public string HashedCohortReference { get; set; }
        public string HashedDraftApprenticeshipId { get; set; }
        public CohortStatus CohortStatus { get; set; }
        public virtual ICollection<Apprenticeship> Apprenticeships { get; set; } = new List<Apprenticeship>();
    }

    public enum CohortStatus
    {
        Unknown,
        Draft,
        Review,
        WithTrainingProvider,
        WithTransferSender,
        Approved
    }
}
