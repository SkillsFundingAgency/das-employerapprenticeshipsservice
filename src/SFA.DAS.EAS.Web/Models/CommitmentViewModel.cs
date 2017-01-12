using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.Models
{
    public class CommitmentViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
    }

    public sealed class CommitmentDetailsViewModel : CommitmentViewModel
    {
        public RequestStatus Status { get; set; }
        public bool ShowApproveOnlyOption { get; set; }
        public string LatestMessage { get; set; }
        public bool HasApprenticeships { get; set; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }
    }
}