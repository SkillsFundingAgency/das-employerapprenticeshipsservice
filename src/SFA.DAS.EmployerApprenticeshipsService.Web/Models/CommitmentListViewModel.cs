using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class CommitmentListViewModel
    {
        public long AccountId { get; set; }
        public List<CommitmentListItem> Commitments { get; set; }
    }
}