using SFA.DAS.Encoding;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Models.Commitments
{
    public class CohortV2
    {
        public long Id { get; set; }
      
        public int? NumberOfDraftApprentices { get; set; }       
        public CohortStatus CohortStatus { get; set; }
        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }
        public string HashedId { get; private set; }
        public void SetHashId(IEncodingService encodingService)
        {
            HashedId = encodingService.Encode(Id, EncodingType.CohortReference);
        }
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
