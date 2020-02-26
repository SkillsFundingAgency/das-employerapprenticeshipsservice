using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleDraftApprenticeship
{
    public class GetSingleDraftApprenticeshipRequest : IAsyncRequest<GetSingleDraftApprenticeshipResponse>
    {
        public long CohortId { get; set; }
    }
}
