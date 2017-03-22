using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EAS.Application.Queries.GetCommitments
{
    public sealed class GetCommitmentsResponse
    {
        public List<CommitmentListItem> Commitments { get; set; }
    }
}
