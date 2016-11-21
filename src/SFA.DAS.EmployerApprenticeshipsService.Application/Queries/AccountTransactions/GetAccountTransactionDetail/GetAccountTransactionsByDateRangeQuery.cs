using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountTransactionsByDateRangeQuery : IAsyncRequest<GetAccountLevyDeclationTransactionsByDateRangeResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}