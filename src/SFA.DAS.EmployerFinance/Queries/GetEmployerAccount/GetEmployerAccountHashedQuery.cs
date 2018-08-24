using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccount
{
    public class GetEmployerAccountHashedQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
    }
}
