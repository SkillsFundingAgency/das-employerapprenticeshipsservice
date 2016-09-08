using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryRequest : IAsyncRequest<GetApprenticeshipQueryResponse>
    {
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}