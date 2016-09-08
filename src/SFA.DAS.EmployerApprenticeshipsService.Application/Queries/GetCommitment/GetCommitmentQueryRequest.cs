using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitment
{
    public class GetCommitmentQueryRequest : IAsyncRequest<GetCommitmentQueryResponse>
    {
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
    }
}