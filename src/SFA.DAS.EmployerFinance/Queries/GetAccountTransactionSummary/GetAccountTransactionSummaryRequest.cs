using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary
{
    public class GetAccountTransactionSummaryRequest : IAsyncRequest<GetAccountTransactionSummaryResponse>
    {
        public string HashedAccountId { get; set; }
    }
}
