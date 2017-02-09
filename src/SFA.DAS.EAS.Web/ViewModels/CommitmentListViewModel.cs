using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CommitmentListViewModel
    {
        public string AccountHashId { get; set; }

        public IEnumerable<CommitmentListItemViewModel> Commitments { get; set; }

        // Page properties

        public string PageTitle { get; set; }

        public string PageId { get; set; }

        public string PageHeading { get; set; }

        public string PageHeading2 { get; set; }
    }
}