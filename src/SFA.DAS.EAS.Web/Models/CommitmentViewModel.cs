using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class CommitmentViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public RequestStatus Status { get; set; }
        public IList<ApprenticeshipViewModel> Apprenticeships { get; set; }
    }
}