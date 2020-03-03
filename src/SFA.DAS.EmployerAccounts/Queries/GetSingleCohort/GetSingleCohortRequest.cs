using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountCohort
{
    public class GetSingleCohortRequest : IAsyncRequest<GetSingleCohortResponse>
    {
        public string HashedAccountId { get; set; }        
        public string ExternalUserId { get; set; }
    }
}
