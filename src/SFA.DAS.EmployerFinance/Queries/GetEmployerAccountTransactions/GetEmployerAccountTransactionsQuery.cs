using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsQuery : IAsyncRequest<GetEmployerAccountTransactionsResponse>
    {
        public string HashedAccountId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ExternalUserId { get; set; }
    }
}
