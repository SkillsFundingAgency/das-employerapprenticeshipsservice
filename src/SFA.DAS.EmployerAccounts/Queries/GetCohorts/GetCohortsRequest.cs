using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetCohorts
{
    public class GetCohortsRequest : IAsyncRequest<GetCohortsResponse>
    {
        public long? AccountId { get; set; }
    }
}
