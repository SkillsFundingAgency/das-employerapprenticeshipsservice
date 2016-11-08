using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountTransactionDetailQuery : IAsyncRequest<GetAccountTransactionDetailResponse>
    {
        public long Id { get; set; }
    }
}