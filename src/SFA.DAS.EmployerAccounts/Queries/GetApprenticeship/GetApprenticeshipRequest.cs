using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipRequest : IAsyncRequest<GetApprenticeshipResponse>
    {
        public long? AccountId { get; set; }

    }
}
