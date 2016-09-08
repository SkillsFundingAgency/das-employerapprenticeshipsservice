using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitments
{
    public sealed class GetCommitmentsResponse
    {
        public List<CommitmentListItem> Commitments { get; set; }
    }
}
