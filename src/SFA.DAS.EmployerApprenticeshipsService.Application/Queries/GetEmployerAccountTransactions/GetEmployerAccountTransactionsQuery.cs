using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsQuery : IAsyncRequest<GetEmployerAccountTransactionsResponse>
    {
        public long AccountId { get; set; }
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
