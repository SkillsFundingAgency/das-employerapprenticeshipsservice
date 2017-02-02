using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class YourCohortsViewModel
    {
        public int WaitingToBeSentCount { get; set; }

        public int ReadyForApprovalCount { get; set; }

        public int ReadyForReviewCount { get; set; }

        public int WithProviderCount { get; set; }
    }

    public class CommitmentListViewModel2
    {
        public string AccountHashId { get; set; }

        public IEnumerable<CommitmentListItemViewModel> Commitments { get; set; }

        public bool HasSignedAgreement { get; set; }

        // Page properties

        public string PageTitle { get; set; }

        public string PageId { get; set; }

        public string PageHeading { get; set; }

        public string PageHeading2 { get; set; }
    }
}