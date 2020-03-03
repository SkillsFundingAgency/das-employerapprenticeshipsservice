using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Models.Commitments
{
    public class CohortV2
    {
        public long Id { get; set; }
        public string HashedId { get; set; }
        public int? NumberOfDraftApprentices { get; set; }       
        public CohortStatus CohortStatus { get; set; }
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }
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
