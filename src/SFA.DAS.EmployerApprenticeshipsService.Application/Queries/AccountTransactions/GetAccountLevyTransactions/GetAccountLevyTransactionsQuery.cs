using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountLevyTransactions
{
    public class GetAccountLevyTransactionsQuery : IAsyncRequest<GetAccountLevyTransactionsResponse>
    {
        public long AccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ExternalUserId { get; set; }
    }
}
