using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountCohort
{
    public class GetAccountCohortRequest : IAsyncRequest<GetAccountCohortResponse>
    {
        public string HashedAccountId { get; set; }
        public long? AccountId { get; set; }
    }
}
