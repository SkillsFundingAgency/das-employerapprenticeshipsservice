using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.Models
{
    public class CommitmentListViewModel
    {
        public string AccountHashId { get; set; }
        public List<CommitmentListItemViewModel> Commitments { get; set; }
        public int NumberOfTasks { get; set; }
    }
}